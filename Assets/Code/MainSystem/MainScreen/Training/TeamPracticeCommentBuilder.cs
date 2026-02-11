using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 팀 연습 결과 코멘트를 생성하는 빌더
    /// 
    /// 책임:
    /// - UnitDataSO + ResultData로부터 CommentData 생성
    /// - 스탯 변화 정보 수집 및 포맷팅
    /// - CommentManager에 코멘트 등록
    /// 
    /// SRP: 코멘트 생성 로직만 담당
    /// </summary>
    public class TeamPracticeCommentBuilder : MonoBehaviour
    {
        [Header("Icons")]
        [SerializeField] private AssetReferenceSprite conditionSprite;

        [Header("Dependencies")]
        [SerializeField] private StatManager statManager;

        // 캐시된 스프라이트
        private Sprite _cachedConditionSprite;

        #region Validation

        private async void Awake()
        {
            await LoadConditionSprite();
            ValidateDependencies();
        }

        private async Task LoadConditionSprite()
        {
            if (conditionSprite == null)
            {
                Debug.LogWarning("[CommentBuilder] Condition sprite reference is missing");
                return;
            }

            try
            {
                _cachedConditionSprite = await conditionSprite.LoadAssetAsync<Sprite>().Task;
                Debug.Log("[CommentBuilder] Condition sprite loaded successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CommentBuilder] Failed to load condition sprite: {e.Message}");
            }
        }

        private void ValidateDependencies()
        {
            if (_cachedConditionSprite == null)
                Debug.LogWarning("[CommentBuilder] Condition sprite is not loaded");

            if (statManager == null)
                statManager = StatManager.Instance;
        }

        #endregion

        #region Public API

        /// <summary>
        /// 특정 멤버에 대한 팀 연습 코멘트 생성 및 등록
        /// </summary>
        public void BuildCommentForMember(UnitDataSO unit, TeamPracticeResultData resultData)
        {
            if (!ValidateInputs(unit, resultData)) return;

            var commentData = CreateComment(unit, resultData);
            
            if (commentData != null)
            {
                CommentManager.Instance.AddComment(commentData);
                Debug.Log($"[CommentBuilder] Comment created for {unit.unitName}");
            }
        }

        /// <summary>
        /// 여러 멤버에 대한 코멘트 일괄 생성
        /// </summary>
        public void BuildCommentsForAllMembers(TeamPracticeResultData resultData)
        {
            if (resultData == null || !resultData.IsValid())
            {
                Debug.LogError("[CommentBuilder] Invalid result data");
                return;
            }

            foreach (var unit in resultData.selectedMembers)
            {
                BuildCommentForMember(unit, resultData);
            }
        }

        #endregion

        #region Comment Creation

        private bool ValidateInputs(UnitDataSO unit, TeamPracticeResultData resultData)
        {
            if (unit == null)
            {
                Debug.LogError("[CommentBuilder] Unit is null");
                return false;
            }

            if (resultData == null || !resultData.IsValid())
            {
                Debug.LogError("[CommentBuilder] Result data is invalid");
                return false;
            }

            return true;
        }

        /// <summary>
        /// CommentData 객체 생성
        /// </summary>
        private CommentData CreateComment(UnitDataSO unit, TeamPracticeResultData resultData)
        {
            // 코멘트 텍스트 소스 선택
            var commentDataSO = GetCommentSource(unit, resultData.isSuccess);
            if (commentDataSO == null)
            {
                Debug.LogWarning($"[CommentBuilder] No comment data found for {unit.unitName}");
                return null;
            }

            // 스탯 변화 정보 수집
            var statChanges = CollectStatChanges(unit, resultData);

            // CommentData 생성
            return new CommentData(
                title: $"{unit.unitName}의 팀 훈련일지",
                content: commentDataSO.comment,
                statChanges: statChanges,
                trainingType: PracticenType.Team, // Team 타입으로 변경 (기존 Personal은 오타로 보임)
                thoughts: commentDataSO.thoughts,
                memberName: unit.unitName
            );
        }

        /// <summary>
        /// 성공/실패에 따른 코멘트 소스 선택
        /// </summary>
        private PracticeCommentDataSO GetCommentSource(UnitDataSO unit, bool isSuccess)
        {
            return isSuccess ? unit.successComment : unit.failComment;
        }

        #endregion

        #region Stat Changes Collection

        /// <summary>
        /// 스탯 변화 정보 수집
        /// 순서: 컨디션 -> 팀 스탯 -> 개인 스탯들
        /// </summary>
        private List<StatChangeInfo> CollectStatChanges(UnitDataSO unit, TeamPracticeResultData resultData)
        {
            var statChanges = new List<StatChangeInfo>();

            // 1. 컨디션 변화 (항상 표시)
            AddConditionChange(statChanges, resultData);

            // 2. 팀 스탯 변화 (성공 시만)
            if (resultData.isSuccess)
            {
                AddTeamStatChange(statChanges, resultData);
            }

            // 3. 개인 스탯 변화들 (증가분이 있는 것만)
            AddMemberStatChanges(statChanges, unit, resultData);

            return statChanges;
        }

        /// <summary>
        /// 컨디션 변화 추가
        /// </summary>
        private void AddConditionChange(List<StatChangeInfo> statChanges, TeamPracticeResultData resultData)
        {
            int conditionDelta = Mathf.RoundToInt(resultData.teamConditionDelta);
            
            statChanges.Add(new StatChangeInfo(
                statName: "컨디션",
                delta: conditionDelta,
                icon: _cachedConditionSprite
            ));
        }

        /// <summary>
        /// 팀 스탯 변화 추가
        /// </summary>
        private void AddTeamStatChange(List<StatChangeInfo> statChanges, TeamPracticeResultData resultData)
        {
            if (resultData.teamStat == null)
            {
                Debug.LogWarning("[CommentBuilder] Team stat is null");
                return;
            }

            var teamBaseStat = statManager.GetTeamStat(resultData.teamStat.statType);
            if (teamBaseStat == null)
            {
                Debug.LogError($"[CommentBuilder] Cannot find team stat: {resultData.teamStat.statType}");
                return;
            }

            statChanges.Add(new StatChangeInfo(
                statName: resultData.teamStat.statName,
                delta: resultData.teamStatDelta,
                icon: teamBaseStat.StatIcon
            ));
        }

        /// <summary>
        /// 멤버 개인 스탯 변화들 추가
        /// </summary>
        private void AddMemberStatChanges(
            List<StatChangeInfo> statChanges,
            UnitDataSO unit,
            TeamPracticeResultData resultData)
        {
            if (unit.stats == null) return;

            foreach (var stat in unit.stats)
            {
                int delta = resultData.GetStatDelta(unit.memberType, stat.statType);
                
                // 증가분이 없으면 스킵
                if (delta <= 0) continue;

                var baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                if (baseStat == null)
                {
                    Debug.LogWarning($"[CommentBuilder] Cannot find stat: {unit.memberType}.{stat.statType}");
                    continue;
                }

                statChanges.Add(new StatChangeInfo(
                    statName: stat.statName,
                    delta: delta,
                    icon: baseStat.StatIcon
                ));
            }
        }

        #endregion

        #region Unity Lifecycle

        private void OnDestroy()
        {
            // Addressable 에셋 해제
            if (conditionSprite != null && conditionSprite.IsValid())
            {
                conditionSprite.ReleaseAsset();
            }
        }

        #endregion

        #region Debug

        /// <summary>
        /// 디버그용: 코멘트 미리보기
        /// </summary>
        [ContextMenu("Test Comment Generation")]
        public void TestCommentGeneration()
        {
            Debug.Log("[CommentBuilder] This is a test context menu. Attach actual data in Inspector to test.");
        }

        #endregion
    }
}