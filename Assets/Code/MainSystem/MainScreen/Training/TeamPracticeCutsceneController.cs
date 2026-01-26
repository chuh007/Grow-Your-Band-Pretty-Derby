using System.Collections.Generic;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamPracticeCutsceneController : MonoBehaviour
    {
        public PlayableDirector director;
        public TimelineAsset timelineAsset;

        [System.Serializable]
        public class MemberObjectBinding
        {
            public MemberType memberType;
            public GameObject characterObject;
        }

        [Header("Member Bindings")]
        public List<MemberObjectBinding> memberObjects;

        [Header("Result Window")]
        public PracticeResultWindow resultWindow;

        [Header("Icons")]
        [SerializeField] private Sprite conditionSprite;

        private void Start()
        {
            ApplyTimelineBindings();
            director.stopped += OnTimelineEnd;
            director.Play();
        }

        private void ApplyTimelineBindings()
        {
            var selected = TeamPracticeResultCache.SelectedMembers;

            foreach (var binding in memberObjects)
                binding.characterObject.SetActive(false);

            foreach (var unit in selected)
            {
                var binding = memberObjects.Find(b => b.memberType == unit.memberType);
                if (binding == null) continue;

                binding.characterObject.SetActive(true);
                var track = FindTrackByName(binding.memberType.ToString());
                if (track != null)
                    director.SetGenericBinding(track, binding.characterObject);
            }
        }

        private TrackAsset FindTrackByName(string name)
        {
            foreach (var track in timelineAsset.GetOutputTracks())
                if (track.name == name)
                    return track;

            return null;
        }

        private async void OnTimelineEnd(PlayableDirector director)
        {
            foreach (var binding in memberObjects)
            {
                if (!binding.characterObject.activeInHierarchy) continue;

                var animator = binding.characterObject.GetComponent<Animator>();
                if (animator == null) continue;

                string animName = GetResultAnimationName(binding.memberType);
                animator.Play(animName, 0, 0f);
            }

            await UniTask.Delay(1000);

            foreach (var unit in TeamPracticeResultCache.SelectedMembers)
            {
                AddCommentForMember(unit);
                await ShowResultForMember(unit);
            }
            
            Debug.Log("[TeamPracticeCutsceneController] 모든 결과 확인 완료, 메인 씬으로 이동");
            await UniTask.Delay(500); 
            SceneManager.LoadScene("Lch");
        }

        private void AddCommentForMember(UnitDataSO unit)
        {
            bool isSuccess = TeamPracticeResultCache.IsSuccess;
            var commentDataSO = isSuccess ? unit.teamSuccessComment : unit.teamFailComment;

            if (commentDataSO == null)
            {
                Debug.LogWarning($"Team practice comment data is missing for {unit.memberType}");
                return;
            }
            var statChanges = new List<StatChangeInfo>();
            
            float conditionDelta = TeamPracticeResultCache.TeamConditionDelta;
            statChanges.Add(new StatChangeInfo("컨디션", Mathf.RoundToInt(conditionDelta), conditionSprite));
            
            if (isSuccess && TeamPracticeResultCache.TeamStat != null)
            {
                float teamStatDelta = TeamPracticeResultCache.TeamStatDelta;
                statChanges.Add(new StatChangeInfo(
                    TeamPracticeResultCache.TeamStat.statName, 
                    Mathf.RoundToInt(teamStatDelta), 
                    TeamPracticeResultCache.TeamStat.statIcon
                ));
            }
            
            foreach (var stat in unit.stats)
            {
                var key = (unit.memberType, stat.statType);
                if (TeamPracticeResultCache.StatDeltaDict.TryGetValue(key, out int delta) && delta > 0)
                {
                    BaseStat baseStat = StatManager.Instance.GetMemberStat(unit.memberType, stat.statType);
                    if (baseStat != null)
                    {
                        statChanges.Add(new StatChangeInfo(stat.statName, delta, baseStat.StatIcon));
                    }
                }
            }
            var comment = new CommentData(
                $"{unit.unitName}의 팀 훈련일지",
                commentDataSO.comment,
                statChanges,
                PracticenType.Personal,
                commentDataSO.icon,
                isSuccess,
                commentDataSO.thoughts,
                unit.unitName 
            );

            CommentManager.instance.AddComment(comment,true); 
        }

        private async UniTask ShowResultForMember(UnitDataSO unit)
        {
            var statDeltaDict = new Dictionary<(MemberType, StatType), int>();
            
            foreach (var stat in unit.stats)
            {
                var key = (unit.memberType, stat.statType);
                if (TeamPracticeResultCache.StatDeltaDict.TryGetValue(key, out int delta))
                {
                    statDeltaDict[key] = delta;
                }
            }
            
            await resultWindow.Play(
                StatManager.Instance,
                new List<UnitDataSO> { unit },
                TeamPracticeResultCache.TeamConditionCurrent,
                TeamPracticeResultCache.TeamConditionDelta,
                TeamPracticeResultCache.TeamStat,
                TeamPracticeResultCache.TeamStatDelta,
                statDeltaDict,
                TeamPracticeResultCache.IsSuccess,
                null
            );
        }

        private string GetResultAnimationName(MemberType member)
        {
            var unit = TeamPracticeResultCache.SelectedMembers.Find(m => m.memberType == member);
            if (unit == null) return "Idle";

            return TeamPracticeResultCache.IsSuccess ? "Succse" : "Faill";
        }

        public void OnClickReturnToMain()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}