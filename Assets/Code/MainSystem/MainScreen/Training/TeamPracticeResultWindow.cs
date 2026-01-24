using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamPracticeResultWindow : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private StatResultUI statUI;

        [Header("Comment Page")]
        [SerializeField] private GameObject statPage;
        [SerializeField] private RectTransform statPageRect;
        [SerializeField] private GameObject commentPageGO;
        [SerializeField] private PracticeCommentPage commentPage;

        [Header("Icons")]
        [SerializeField] private Sprite statIcon;
        [SerializeField] private Sprite gradeIcon;

        private bool skipRequested;

        private void Awake()
        {
            gameObject.SetActive(false);
            statPage.SetActive(false);
            commentPageGO.SetActive(false);
        }

        public async UniTask PlayForTeam()
        {
            var units = TeamPracticeResultCache.SelectedMembers;
            var statDeltaDict = TeamPracticeResultCache.StatDeltaDict;
            var teamStat = TeamPracticeResultCache.TeamStat;
            var teamStatDelta = TeamPracticeResultCache.TeamStatDelta;
            var teamConditionCurrent = TeamPracticeResultCache.TeamConditionBefore;
            var teamConditionDelta = TeamPracticeResultCache.TeamConditionDelta;

            if (units == null || units.Count == 0)
            {
                Debug.LogError("[TeamPracticeResult] No members found.");
                return;
            }

            gameObject.SetActive(true);

            foreach (var unit in units)
            {
                if (unit == null || unit.stats == null)
                {
                    Debug.LogError($"[TeamPracticeResult] Unit or stats is null: {unit?.memberType}");
                    continue;
                }

                skipRequested = false;
                statPage.SetActive(true);
                commentPageGO.SetActive(false);
                statPageRect.anchoredPosition = Vector2.zero;

                List<StatChangeResult> results = new()
                {
                    new StatChangeResult("컨디션", statIcon, gradeIcon,
                        Mathf.RoundToInt(teamConditionCurrent), teamConditionDelta)
                };

                if (teamStat != null)
                {
                    results.Add(new StatChangeResult(teamStat.statName, statIcon, gradeIcon, teamStat.currentValue, teamStatDelta));
                }

                foreach (var stat in unit.stats)
                {
                    var key = (unit.memberType, stat.statType);
                    statDeltaDict.TryGetValue(key, out int delta);

                    BaseStat baseStat = StatManager.Instance.GetMemberStat(unit.memberType, stat.statType);
                    if (baseStat == null)
                    {
                        Debug.LogWarning($"[MissingStat] {unit.memberType}/{stat.statType}");
                        continue;
                    }

                    results.Add(new StatChangeResult(stat.statName, statIcon, gradeIcon, baseStat.CurrentValue, delta));
                }

                await statUI.ShowStats(results);
                await UniTask.Delay(300);

                var tcs = new UniTaskCompletionSource();
                statPageRect.DOAnchorPosX(-1920f, 0.65f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => tcs.TrySetResult());
                await tcs.Task;

                statPage.SetActive(false);
                commentPageGO.SetActive(true);

                var commentList = PracticeResultFactory.BuildCommentData(new List<UnitDataSO> { unit }, statDeltaDict);
                await commentPage.ShowComments(commentList, unit.memberType.ToString());

                await UniTask.WaitUntil(() => skipRequested);
            }

            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            skipRequested = true;
        }
    }
}
