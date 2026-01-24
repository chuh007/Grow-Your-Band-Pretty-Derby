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

        private bool skipRequested = false;

        private void Awake()
        {
            gameObject.SetActive(false);
            commentPageGO.SetActive(false);
            statPage.SetActive(false);
        }

        public async UniTask PlayForTeam(
            StatManager statManager,
            List<UnitDataSO> members,
            Dictionary<(MemberType memberType, StatType statType), int> statDeltaDict
        )
        {
            gameObject.SetActive(true);
            skipRequested = false;

            for (int i = 0; i < members.Count; i++)
            {
                if (skipRequested) break;

                var unit = members[i];
                
                List<StatChangeResult> statResults = BuildStatChangeResults(unit, statManager, statDeltaDict);
                statPage.SetActive(true);
                commentPageGO.SetActive(false);
                statPageRect.anchoredPosition = Vector2.zero;

                await statUI.ShowStats(statResults);
                await UniTask.Delay(300);

                var tcs = new UniTaskCompletionSource();
                statPageRect.DOAnchorPosX(-1920f, 0.65f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => tcs.TrySetResult());
                await tcs.Task;
                
                statPage.SetActive(false);
                commentPageGO.SetActive(true);
                var commentDataList = PracticeResultFactory.BuildCommentData(new List<UnitDataSO> { unit }, statDeltaDict);
                await commentPage.ShowComments(commentDataList, unit.memberType.ToString());

                await UniTask.WaitUntil(() => skipRequested);
            }

            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            skipRequested = true;
        }

        private List<StatChangeResult> BuildStatChangeResults(UnitDataSO unit, StatManager statManager, Dictionary<(MemberType, StatType), int> statDeltaDict)
        {
            var results = new List<StatChangeResult>();

            foreach (var stat in unit.stats)
            {
                var statKey = (unit.memberType, stat.statType);
                statDeltaDict.TryGetValue(statKey, out int delta);

                BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                results.Add(new StatChangeResult(stat.statName, null, null, baseStat.CurrentValue, delta));
            }

            return results;
        }
    }
}
