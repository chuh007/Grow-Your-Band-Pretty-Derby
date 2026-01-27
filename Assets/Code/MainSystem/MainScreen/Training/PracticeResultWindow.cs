using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine.EventSystems;
using System.Threading;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeResultWindow : MonoBehaviour, IPointerDownHandler
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

        [Header("Skip Settings")]
        [SerializeField] private bool allowSkip = true;

        private bool _isPlaying = false;
        private CancellationTokenSource _skipCTS;
        private Tween _currentTween;

        private void Awake()
        {
            gameObject.SetActive(false);
            commentPageGO.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPlaying && allowSkip)
            {
                _skipCTS?.Cancel();
                
                if (_currentTween != null && _currentTween.IsActive())
                {
                    _currentTween.Complete(true);
                }
            }
        }

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
            PersonalpracticeDataSO practiceData = null 
        )
        {
            _isPlaying = true;
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
            _skipCTS = new CancellationTokenSource();
            
            try
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
                
                results.Add(new StatChangeResult(
                    "컨디션", 
                    test2, 
                    test, 
                    Mathf.RoundToInt(conditionCurrent), 
                    conditionDelta
                ));
                
                if (teamStat != null)
                {
                    results.Add(new StatChangeResult(
                        teamStat.statName, 
                        test2, 
                        test, 
                        Mathf.RoundToInt(teamStatCurrentValue), 
                        teamStatDelta                           
                    ));
                }
                
                foreach (var unit in allUnits)
                {
                    foreach (var stat in unit.stats)
                    {
                        BaseStat baseStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                        if (baseStat == null) continue;
                        
                        statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta);
                        
                        results.Add(new StatChangeResult(
                            stat.statName, 
                            test2, 
                            test, 
                            baseStat.CurrentValue, 
                            delta
                        ));
                    }
                }
                
                await statUI.ShowStats(results).AttachExternalCancellation(_skipCTS.Token);
                
                var tcs = new UniTaskCompletionSource();
                _currentTween = statPageRect.DOAnchorPosX(-1920f, 0.65f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => tcs.TrySetResult());
                
                await tcs.Task.AttachExternalCancellation(_skipCTS.Token);

                statPage.SetActive(false);
                commentPageGO.SetActive(true);
                
                await CommentManager.instance.ShowAllComments().AttachExternalCancellation(_skipCTS.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                statPage.SetActive(false);
                commentPageGO.SetActive(false);
                gameObject.SetActive(false);
                _isPlaying = false;
            }
        }

        private void OnDestroy()
        {
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
        }
    }
}