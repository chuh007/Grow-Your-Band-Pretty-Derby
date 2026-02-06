using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamPracticeCutsceneController : MonoBehaviour, IPointerDownHandler
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

        [Header("Skip Settings")]
        [SerializeField] private bool allowSkip = true;

        private bool _isTimelinePlaying = true;
        private bool _hasSkipped = false;

        private void Start()
        {
            ApplyTimelineBindings();
            director.stopped += OnTimelineEnd;
            director.Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isTimelinePlaying && allowSkip && !_hasSkipped)
            {
                SkipTimeline();
            }
        }

        private void SkipTimeline()
        {
            if (!_isTimelinePlaying || _hasSkipped) return;
            
            _hasSkipped = true;
            Debug.Log("[TeamPracticeCutsceneController] Timeline skipped by user");
            
            director.time = director.duration;
            director.Evaluate();
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
            _isTimelinePlaying = false;

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
                CommentManager.instance.ClearAllComments();
                AddCommentForMember(unit);
                CommentManager.instance.SetupComments();
                await ShowResultForMember(unit);
            }
            
            Debug.Log("[TeamPracticeCutsceneController] 모든 결과 확인 완료");
            
            Debug.Log("[TeamPracticeCutsceneController] Raising CheckTurnEnd event");
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
            
            await UniTask.Delay(500);
            
            Debug.Log("[TeamPracticeCutsceneController] Moving to main scene");
            SceneManager.LoadScene("Lch");
        }

        private void AddCommentForMember(UnitDataSO unit)
        {
            bool isSuccess = TeamPracticeResultCache.IsSuccess;
            var commentDataSO = isSuccess ? unit.successComment : unit.failComment;

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
                
                BaseStat teamBaseStat = StatManager.Instance.GetTeamStat(TeamPracticeResultCache.TeamStat.statType);
                statChanges.Add(new StatChangeInfo(
                    TeamPracticeResultCache.TeamStat.statName, 
                    Mathf.RoundToInt(teamStatDelta), 
                    teamBaseStat.StatIcon
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
                commentDataSO.thoughts,
                unit.unitName 
            );

            CommentManager.instance.AddComment(comment, true); 
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
            
            float currentTeamStatValue = StatManager.Instance.GetTeamStat(StatType.TeamHarmony).CurrentValue;
            
            Debug.Log($"[ShowResultForMember] Unit: {unit.unitName}");
            Debug.Log($"[ShowResultForMember] TeamStatDelta from Cache: {TeamPracticeResultCache.TeamStatDelta}");
            Debug.Log($"[ShowResultForMember] Current TeamStat Value: {currentTeamStatValue}");
            
            await resultWindow.Play(
                StatManager.Instance,
                new List<UnitDataSO> { unit },
                TeamPracticeResultCache.TeamConditionCurrent,
                TeamPracticeResultCache.TeamConditionDelta,
                TeamPracticeResultCache.TeamStat,
                currentTeamStatValue, 
                TeamPracticeResultCache.TeamStatDelta, 
                statDeltaDict,
                TeamPracticeResultCache.IsSuccess,
                null,
                true 
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

        private void OnDestroy()
        {
            director.stopped -= OnTimelineEnd;
        }
    }
}