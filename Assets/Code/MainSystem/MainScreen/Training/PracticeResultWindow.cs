using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine.EventSystems;
using System.Threading;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.Core.Bus.GameEvents.TurnEvents;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 훈련 결과를 표시하는 윈도우
    /// 
    /// 주요 기능:
    /// - 스탯 변화 게이지 애니메이션
    /// - 코멘트 타이핑 애니메이션
    /// - 이전 코멘트 미리보기 (페이지 넘김 효과)
    /// - 클릭/타임아웃으로 스킵 가능
    /// 
    /// 주의사항:
    /// - CommentManager.SetupComments() 호출 후 사용
    /// - 팀 훈련 시 isTeamTraining=true 필수
    /// </summary>
    public class PracticeResultWindow : MonoBehaviour, IPointerDownHandler
    {
        #region Serialized Fields

        [Header("Prefabs")]
        [SerializeField] private GameObject statItemPrefab;
        [SerializeField] private GameObject commentItemPrefab;

        [Header("Parents")]
        [SerializeField] private Transform statParent;
        [SerializeField] private Transform commentParent;
        
        [Header("Icons")]
        [SerializeField] private Sprite conditionIcon;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Skip Settings")]
        [SerializeField] private bool allowSkip = true;
        [SerializeField] private float skipCooldown = 0.3f;
        [SerializeField] private float autoSkipTimeout = 15f;

        [Header("Page Flip Settings")]
        [SerializeField] private CanvasGroup commentCanvasGroup;
        [SerializeField] private float previousPageDisplayDuration = 2f;
        [SerializeField] private float pageTransitionDuration = 0.5f;

        #endregion

        #region Private Fields

        private bool _isPlaying = false;
        private CancellationTokenSource _skipCTS;
        private bool _hasSkipped = false;
        private float _lastSkipTime = 0f;
        private bool _isTeamTraining = false;
        private List<UnitDataSO> _currentUnits;

        private readonly List<GameObject> _spawnedStats = new();
        private readonly List<GameObject> _spawnedComments = new();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            gameObject.SetActive(false);
            ValidateDependencies();
        }

        private void OnDestroy()
        {
            CleanupCancellationToken();
        }

        #endregion

        #region Validation

        private void ValidateDependencies()
        {
            if (statItemPrefab == null)
                Debug.LogError("[PracticeResultWindow] Stat item prefab is missing!");
            
            if (commentItemPrefab == null)
                Debug.LogError("[PracticeResultWindow] Comment item prefab is missing!");
            
            if (statParent == null)
                Debug.LogError("[PracticeResultWindow] Stat parent is missing!");
            
            if (commentParent == null)
                Debug.LogError("[PracticeResultWindow] Comment parent is missing!");
            
            if (conditionIcon == null)
                Debug.LogWarning("[PracticeResultWindow] Condition icon is missing!");
            
            if (commentCanvasGroup == null)
                Debug.LogWarning("[PracticeResultWindow] Comment canvas group is missing - page flip will be disabled!");
        }

        #endregion

        #region Skip Handling

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanSkip())
            {
                Debug.Log($"[PracticeResultWindow] Skip blocked (cooldown or conditions not met)");
                return;
            }

            Debug.Log("[PracticeResultWindow] Skip via OnPointerDown");
            TriggerSkip();
        }

        private bool CanSkip()
        {
            return _isPlaying 
                && allowSkip 
                && !_hasSkipped 
                && (Time.time - _lastSkipTime >= skipCooldown);
        }

        private void TriggerSkip()
        {
            if (_hasSkipped)
            {
                Debug.LogWarning("[PracticeResultWindow] Skip already triggered");
                return;
            }
            
            _hasSkipped = true;
            _lastSkipTime = Time.time;
            
            Debug.Log("[PracticeResultWindow] Skip triggered!");
            
            // 모든 코멘트 애니메이션 즉시 완료
            SkipAllCommentAnimations();
            
            // 비동기 작업 취소
            _skipCTS?.Cancel();
        }

        private void SkipAllCommentAnimations()
        {
            foreach (var commentGO in _spawnedComments)
            {
                if (commentGO == null) continue;

                var commentUI = commentGO.GetComponent<PracticeCommentItemUI>();
                if (commentUI != null && commentUI._currentData != null)
                {
                    commentUI.SkipToComplete(commentUI._currentData);
                }
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// 개인 훈련 결과 표시 (편의 메서드)
        /// </summary>
        public async UniTask ShowPersonalPracticeResult(
            UnitDataSO unit,
            PersonalpracticeDataSO practiceData,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatCurrentValue,
            float teamStatDelta,
            Dictionary<(MemberType, StatType), int> statDeltaDict,
            bool hadAnyStatChanged)
        {
            await Play(
                StatManager.Instance,
                new List<UnitDataSO> { unit },
                conditionCurrent,
                conditionDelta,
                teamStat,
                teamStatCurrentValue,
                teamStatDelta,
                statDeltaDict,
                hadAnyStatChanged,
                practiceData,
                isTeamTraining: false
            );
        }

        /// <summary>
        /// 팀 훈련 결과 표시 (편의 메서드)
        /// </summary>
        public async UniTask ShowTeamPracticeResult(
            UnitDataSO unit,
            TeamPracticeResultData resultData)
        {
            if (resultData == null || !resultData.IsValid())
            {
                Debug.LogError("[PracticeResultWindow] Invalid result data!");
                return;
            }

            var statDeltas = new Dictionary<(MemberType, StatType), int>();
            foreach (var stat in unit.stats)
            {
                int delta = resultData.GetStatDelta(unit.memberType, stat.statType);
                if (delta != 0)
                {
                    statDeltas[(unit.memberType, stat.statType)] = delta;
                }
            }

            await Play(
                StatManager.Instance,
                new List<UnitDataSO> { unit },
                resultData.teamConditionCurrent,
                resultData.teamConditionDelta,
                resultData.teamStat,
                StatManager.Instance.GetTeamStat(StatType.TeamHarmony).CurrentValue,
                resultData.teamStatDelta,
                statDeltas,
                resultData.isSuccess,
                practiceData: null,
                isTeamTraining: true
            );
        }

        /// <summary>
        /// 훈련 결과 표시 (메인 메서드)
        /// </summary>
        public async UniTask Play(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatCurrentValue,
            float teamStatDelta,
            Dictionary<(MemberType memberType, StatType statType), int> statDeltaDict,
            bool hadAnyStatChanged,
            PersonalpracticeDataSO practiceData = null,
            bool isTeamTraining = false)
        {
            // 입력 검증
            if (!ValidatePlayInputs(statManager, allUnits, statDeltaDict))
            {
                return;
            }

            Debug.Log($"[PracticeResultWindow] Play started (Team: {isTeamTraining})");
            
            // 초기화
            InitializePlaySession(allUnits, isTeamTraining);

            try
            {
                gameObject.SetActive(true);
                ClearAll();
                
                // 타이틀 설정
                SetTitle(allUnits);
                
                // 스탯 변화 표시
                await CreateChangedStats(
                    statManager,
                    allUnits,
                    conditionCurrent,
                    conditionDelta,
                    teamStat,
                    teamStatCurrentValue,
                    teamStatDelta,
                    statDeltaDict
                );
                
                // 코멘트 표시 (페이지 넘김 포함)
                await ShowCommentsWithPageFlip();
                
                // 클릭 대기
                await WaitForClick();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[PracticeResultWindow] Skipped by user");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PracticeResultWindow] Error during play: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                EndWindow();
            }
            
            Debug.Log("[PracticeResultWindow] Play completed");
        }

        #endregion

        #region Validation & Initialization

        private bool ValidatePlayInputs(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            Dictionary<(MemberType, StatType), int> statDeltaDict)
        {
            if (statManager == null)
            {
                Debug.LogError("[PracticeResultWindow] StatManager is null!");
                return false;
            }

            if (allUnits == null || allUnits.Count == 0)
            {
                Debug.LogError("[PracticeResultWindow] Units list is null or empty!");
                return false;
            }

            if (statDeltaDict == null)
            {
                Debug.LogWarning("[PracticeResultWindow] Stat delta dictionary is null!");
                // null이어도 진행 가능 (변화 없는 경우)
            }

            return true;
        }

        private void InitializePlaySession(List<UnitDataSO> allUnits, bool isTeamTraining)
        {
            _isPlaying = true;
            _hasSkipped = false;
            _isTeamTraining = isTeamTraining;
            _currentUnits = allUnits;
            
            CleanupCancellationToken();
            _skipCTS = new CancellationTokenSource();
        }

        private void SetTitle(List<UnitDataSO> allUnits)
        {
            if (titleText == null || allUnits == null || allUnits.Count == 0)
                return;

            string unitName = allUnits[0]?.unitName ?? "Unknown";
            titleText.text = $"{unitName} 훈련 결과";
        }

        #endregion

        #region Stat Display

        private async UniTask CreateChangedStats(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatCurrentValue,
            float teamStatDelta,
            Dictionary<(MemberType, StatType), int> statDeltaDict)
        {
            // 1. 컨디션 (변화가 있을 때만)
            if (!Mathf.Approximately(conditionDelta, 0f))
            {
                await CreateStatItemWithGauge(
                    "컨디션", 
                    Mathf.RoundToInt(conditionCurrent), 
                    conditionDelta, 
                    conditionIcon, 
                    baseStat: null
                );
            }
            
            // 2. 팀 스탯 (변화가 있을 때만)
            if (teamStat != null && !Mathf.Approximately(teamStatDelta, 0f))
            {
                BaseStat baseStat = statManager.GetTeamStat(StatType.TeamHarmony);
                if (baseStat != null)
                {
                    await CreateStatItemWithGauge(
                        teamStat.statName, 
                        Mathf.RoundToInt(teamStatCurrentValue), 
                        teamStatDelta, 
                        baseStat.CurrentRankIcon, 
                        baseStat
                    );
                }
                else
                {
                    Debug.LogWarning("[PracticeResultWindow] Team stat is null!");
                }
            }
            
            // 3. 개인 스탯들 (변화가 있는 것만)
            if (statDeltaDict != null)
            {
                await CreateMemberStats(statManager, allUnits, statDeltaDict);
            }
        }

        private async UniTask CreateMemberStats(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            Dictionary<(MemberType, StatType), int> statDeltaDict)
        {
            foreach (var unit in allUnits)
            {
                if (unit == null || unit.stats == null)
                {
                    Debug.LogWarning($"[PracticeResultWindow] Unit or stats is null, skipping");
                    continue;
                }

                foreach (var stat in unit.stats)
                {
                    BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                    if (baseStat == null)
                    {
                        Debug.LogWarning($"[PracticeResultWindow] BaseStat is null for {unit.memberType}.{stat.statType}");
                        continue;
                    }

                    var key = (unit.memberType, stat.statType);
                    statDeltaDict.TryGetValue(key, out int delta);

                    // 변화가 있는 것만 표시
                    if (delta != 0)
                    {
                        await CreateStatItemWithGauge(
                            stat.statName, 
                            baseStat.CurrentValue, 
                            delta, 
                            baseStat.CurrentRankIcon, 
                            baseStat
                        );
                    }
                }
            }
        }

        private async UniTask CreateStatItemWithGauge(
            string statName, 
            int currentValue, 
            float delta, 
            Sprite icon, 
            BaseStat baseStat = null)
        {
            if (statItemPrefab == null || statParent == null)
            {
                Debug.LogError("[PracticeResultWindow] Cannot create stat item - prefab or parent is null!");
                return;
            }

            var go = Instantiate(statItemPrefab, statParent);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);

            var statUI = go.GetComponent<StatResultItemUI>();
            if (statUI == null)
            {
                Debug.LogError("[PracticeResultWindow] StatResultItemUI component not found on prefab!");
                Destroy(go);
                return;
            }

            statUI.SetInitialData(statName);
            
            try
            {
                await statUI.AnimateToValue(icon, currentValue, 0.5f, _skipCTS.Token, baseStat);
            }
            catch (OperationCanceledException)
            {
                // 스킵 시 즉시 최종 값 설정
                statUI.ForceSetValue(icon, currentValue, baseStat);
            }
            
            _spawnedStats.Add(go);

            Debug.Log($"[PracticeResultWindow] Created stat: {statName} = {currentValue} (Δ{delta:+0;-0}), HasBaseStat={baseStat != null}");
        }

        #endregion

        #region Comment Display

        private async UniTask ShowCommentsWithPageFlip()
        {
            // 현재 유닛의 이름 가져오기
            string memberName = GetCurrentMemberName();
            if (memberName == null)
            {
                Debug.LogWarning("[PracticeResultWindow] No member name available");
                await CreateCurrentComments();
                return;
            }

            // 이전 코멘트 확인
            var previousComments = CommentManager.Instance?.GetPreviousCommentsByMember(memberName);
            bool hasPreview = previousComments != null && previousComments.Count > 0;

            if (hasPreview)
            {
                Debug.Log($"[PracticeResultWindow] Showing previous comments for {memberName}");
                
                // 이전 코멘트 표시
                await ShowPreviousComments(previousComments);
                
                // 잠시 대기
                await UniTask.Delay(
                    TimeSpan.FromSeconds(previousPageDisplayDuration), 
                    cancellationToken: _skipCTS.Token
                );
                
                // 페이지 넘김 효과
                await PageFlipTransition();
            }
            
            // 현재 코멘트 표시
            await CreateCurrentComments();
        }

        private string GetCurrentMemberName()
        {
            if (_currentUnits == null || _currentUnits.Count == 0)
                return null;
            
            return _currentUnits[0]?.unitName;
        }

        private async UniTask ShowPreviousComments(List<CommentData> previousComments)
        {
            Debug.Log($"[PracticeResultWindow] Showing {previousComments.Count} previous comments");
            
            // Canvas 그룹 보이기
            if (commentCanvasGroup != null)
                commentCanvasGroup.alpha = 1f;

            // 손글씨 폰트 가져오기
            TMP_FontAsset handwritingFont = GetHandwritingFont();

            // 이전 코멘트들 즉시 표시 (애니메이션 없이)
            foreach (var commentData in previousComments)
            {
                var go = CreateCommentUI(commentData, handwritingFont, instant: true);
                if (go != null)
                {
                    _spawnedComments.Add(go);
                }
            }

            await UniTask.Yield();
        }

        private async UniTask PageFlipTransition()
        {
            if (commentCanvasGroup == null)
            {
                Debug.LogWarning("[PracticeResultWindow] No canvas group for page flip - clearing directly");
                ClearComments();
                return;
            }

            Debug.Log("[PracticeResultWindow] Page flip transition starting");
            
            // Fade out
            await FadeCanvasGroup(1f, 0f, pageTransitionDuration);
            
            // 이전 코멘트 삭제
            ClearComments();
            
            // 짧은 대기
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _skipCTS.Token);
            
            // Fade in
            await FadeCanvasGroup(0f, 1f, pageTransitionDuration);
            
            Debug.Log("[PracticeResultWindow] Page flip transition completed");
        }

        private async UniTask FadeCanvasGroup(float from, float to, float duration)
        {
            if (commentCanvasGroup == null) return;

            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                _skipCTS.Token.ThrowIfCancellationRequested();
                
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                commentCanvasGroup.alpha = Mathf.Lerp(from, to, t);
                
                await UniTask.Yield();
            }
            
            commentCanvasGroup.alpha = to;
        }

        private async UniTask CreateCurrentComments()
        {
            if (CommentManager.Instance == null)
            {
                Debug.LogError("[PracticeResultWindow] CommentManager is null!");
                return;
            }

            var setupComments = CommentManager.Instance.GetCurrentComments();
            
            if (setupComments == null || setupComments.Count == 0)
            {
                Debug.Log("[PracticeResultWindow] No current comments to display");
                return;
            }

            TMP_FontAsset handwritingFont = GetHandwritingFont();
            
            Debug.Log($"[PracticeResultWindow] Creating {setupComments.Count} comment groups");

            foreach (var kvp in setupComments)
            {
                _skipCTS.Token.ThrowIfCancellationRequested();

                foreach (var commentData in kvp.Value)
                {
                    var go = CreateCommentUI(commentData, handwritingFont, instant: false);
                    if (go != null)
                    {
                        _spawnedComments.Add(go);
                    }
                }
            }

            await UniTask.Yield();
        }

        private GameObject CreateCommentUI(CommentData commentData, TMP_FontAsset font, bool instant)
        {
            if (commentItemPrefab == null || commentParent == null)
            {
                Debug.LogError("[PracticeResultWindow] Cannot create comment - prefab or parent is null!");
                return null;
            }

            var go = Instantiate(commentItemPrefab, commentParent);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            
            var commentUI = go.GetComponent<PracticeCommentItemUI>();
            if (commentUI == null)
            {
                Debug.LogError("[PracticeResultWindow] PracticeCommentItemUI not found on prefab!");
                Destroy(go);
                return null;
            }
            
            // 폰트 설정
            if (font != null)
            {
                commentUI.SetHandwritingFont(font);
            }
            
            // 코멘트 표시 (즉시 or 애니메이션)
            if (instant)
            {
                commentUI.ShowInstantly(commentData);
            }
            else
            {
                commentUI.Setup(commentData);
            }
            
            return go;
        }

        private TMP_FontAsset GetHandwritingFont()
        {
            if (_currentUnits != null && _currentUnits.Count > 0 && _currentUnits[0] != null)
            {
                return _currentUnits[0].handwritingFont;
            }
            return null;
        }

        #endregion

        #region Wait & Cleanup

        private async UniTask WaitForClick()
        {
            Debug.Log($"[PracticeResultWindow] Waiting for click (timeout: {autoSkipTimeout}s)...");
            
            float waitStartTime = Time.time;
            
            var clickTask = UniTask.WaitUntil(() => _hasSkipped, cancellationToken: _skipCTS.Token);
            var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(autoSkipTimeout), cancellationToken: _skipCTS.Token);
            
            var completedIndex = await UniTask.WhenAny(clickTask, timeoutTask);
            
            if (completedIndex == 1)
            {
                Debug.Log($"[PracticeResultWindow] Auto-skip after {autoSkipTimeout}s timeout");
                _hasSkipped = true;
            }
            else
            {
                float elapsedTime = Time.time - waitStartTime;
                Debug.Log($"[PracticeResultWindow] Click detected after {elapsedTime:F2}s");
            }
        }

        private void ClearComments()
        {
            foreach (var go in _spawnedComments)
            {
                if (go != null)
                    Destroy(go);
            }
            _spawnedComments.Clear();
        }

        private void ClearAll()
        {
            foreach (var go in _spawnedStats)
            {
                if (go != null)
                    Destroy(go);
            }
            _spawnedStats.Clear();

            ClearComments();
        }

        private void EndWindow()
        {
            Debug.Log("[PracticeResultWindow] EndWindow called");
            
            // 현재 코멘트를 이전 코멘트로 저장
            CommentManager.Instance?.SaveCurrentAsPrevious();
            
            // UI 정리
            ClearAll();
            gameObject.SetActive(false);
            
            // 상태 초기화
            _isPlaying = false;
            _hasSkipped = false;
            _currentUnits = null;
            
            // 현재 코멘트 삭제
            CommentManager.Instance?.ClearCurrent();
            
            // 팀 훈련이 아닌 경우만 턴 종료 체크
            if (!_isTeamTraining)
            {
                Debug.Log("[PracticeResultWindow] Raising CheckTurnEnd event");
                Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
            }
            
            _isTeamTraining = false;
            
            Debug.Log("[PracticeResultWindow] EndWindow completed");
        }

        private void CleanupCancellationToken()
        {
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
            _skipCTS = null;
        }

        #endregion
    }
}