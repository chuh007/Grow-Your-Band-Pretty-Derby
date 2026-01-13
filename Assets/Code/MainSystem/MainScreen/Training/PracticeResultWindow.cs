using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeResultWindow : MonoBehaviour
    {
        [SerializeField] private StatResultUI statUI;
        [SerializeField] private Sprite test;
        [SerializeField] private Sprite test2;

        [Header("Comment Page")]
        [SerializeField] private GameObject statPage;
        [SerializeField] private RectTransform statPageRect;
        [SerializeField] private GameObject commentPageGO;
        [SerializeField] private PracticeCommentPage commentPage;

        private void Awake()
        {
            gameObject.SetActive(false);
            commentPageGO.SetActive(false);
        }

        public async UniTask Play(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatDelta,
            Dictionary<(MemberType memberType, StatType statType), int> statDeltaDict,
            bool hadAnyStatChanged)
        {
            gameObject.SetActive(true);
            statPage.SetActive(true);
            commentPageGO.SetActive(false); 

            List<StatChangeResult> results = new();

            results.Add(new StatChangeResult("컨디션", test2, test, Mathf.RoundToInt(conditionCurrent), conditionDelta));
            results.Add(new StatChangeResult(teamStat.statName, test2, test, teamStat.currentValue, teamStatDelta));

            foreach (var unit in allUnits)
            {
                foreach (var stat in unit.stats)
                {
                    BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                    statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta);

                    results.Add(new StatChangeResult(stat.statName, test2, test, baseStat.CurrentValue, delta));
                }
            }

            await statUI.ShowStats(results);

            var tcs = new UniTaskCompletionSource();

            statPageRect.DOAnchorPosX(-1920f, 0.65f)
                .SetEase(Ease.OutBack)         
                .OnComplete(() => tcs.TrySetResult());

            await tcs.Task;

            statPage.SetActive(false);
            commentPageGO.SetActive(true); 

            var commentList = new List<CommentData>();

            foreach (var unit in allUnits)
            {
                List<StatChangeInfo> unitStatChanges = new();

                foreach (var stat in unit.stats)
                {
                    if (!statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta) || delta == 0)
                        continue;

                    BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);

                    var statInfo = new StatChangeInfo(stat.statName, delta, test);
                    unitStatChanges.Add(statInfo);
                }

                if (hadAnyStatChanged)
                {
                    commentList.Add(new CommentData(
                        $"{unit.memberType}의 연습 결과",
                        $"{unit.memberType}의 능력치가 향상되었습니다.",
                        unitStatChanges
                    ));
                }
            }

            if (!hadAnyStatChanged)
            {
                commentList.Add(new CommentData(
                    "변화 없음",
                    "이번 연습에서는 능력치 변화가 없습니다.",
                    new List<StatChangeInfo>()
                ));
            }

            await commentPage.ShowComments(commentList);
        }
    }
}
