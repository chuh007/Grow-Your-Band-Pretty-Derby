using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
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
        
        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

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
            bool hadAnyStatChanged,
            PersonalpracticeDataSO practiceData = null 
        )
        {
            gameObject.SetActive(true);
            statPage.SetActive(true);
            commentPageGO.SetActive(false);

            statPageRect.anchoredPosition = Vector2.zero;
            
            if (titleText != null && allUnits != null && allUnits.Count > 0)
            {
                titleText.text = $"{allUnits[0].unitName} 스탯 변화";
            }

            List<StatChangeResult> results = new();

            results.Add(new StatChangeResult("컨디션", test2, test, Mathf.RoundToInt(conditionCurrent), conditionDelta));
            
            if (teamStat != null)
            {
                results.Add(new StatChangeResult(teamStat.statName, test2, test, teamStat.currentValue, teamStatDelta));
            }

            foreach (var unit in allUnits)
            {
                foreach (var stat in unit.stats)
                {
                    BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                    if (baseStat == null) continue;
                    
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

            await CommentManager.instance.ShowAllComments();
        }
    }
}