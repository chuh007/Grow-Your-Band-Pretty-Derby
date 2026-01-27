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
        [SerializeField] private RectTransform commentPageRect;
        [SerializeField] private PracticeCommentPage commentPage;

        [Header("Canvas Groups")]
        [SerializeField] private CanvasGroup statPageCanvasGroup;
        [SerializeField] private CanvasGroup commentPageCanvasGroup;
        
        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Skip Settings")]
        [SerializeField] private bool allowSkip = true;

        [Header("Transition Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private Ease fadeEase = Ease.OutCubic;

        private bool _isPlaying = false;
        private CancellationTokenSource _skipCTS;
        private Sequence _currentSequence;

        private void Awake()
        {
            gameObject.SetActive(false);
            commentPageGO.SetActive(false);

            statPageCanvasGroup = EnsureCanvasGroup(statPage);
            commentPageCanvasGroup = EnsureCanvasGroup(commentPageGO);

            statPageCanvasGroup.blocksRaycasts = true;
            commentPageCanvasGroup.blocksRaycasts = false;
        }

        private CanvasGroup EnsureCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            return cg != null ? cg : go.AddComponent<CanvasGroup>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPlaying && allowSkip)
            {
                DOTween.KillAll();
                _skipCTS?.Cancel();
                _currentSequence?.Kill();
                statUI.StopAllCoroutines();
                statUI.ClearStats();
                CommentManager.instance.ClearAllComments();
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

            CommentManager.instance.SetupComments();

            try
            {
                gameObject.SetActive(true);
                statPage.SetActive(true);
                commentPageGO.SetActive(false);

                statPageRect.anchoredPosition = Vector2.zero;
                statPageCanvasGroup.alpha = 1f;
                statPageCanvasGroup.blocksRaycasts = true;
                commentPageCanvasGroup.blocksRaycasts = false;

                if (titleText != null && allUnits != null && allUnits.Count > 0)
                {
                    titleText.text = $"{allUnits[0].unitName} 스탯 변화";
                }

                List<StatChangeResult> results = new();
                results.Add(new StatChangeResult("컨디션", test2, test, Mathf.RoundToInt(conditionCurrent), conditionDelta));

                if (teamStat != null)
                {
                    results.Add(new StatChangeResult(teamStat.statName, test2, test, Mathf.RoundToInt(teamStatCurrentValue), teamStatDelta));
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

                try
                {
                    await statUI.ShowStats(results, _skipCTS.Token);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }

                await TransitionToCommentPage().AttachExternalCancellation(_skipCTS.Token);

                await CommentManager.instance.ShowAllComments().AttachExternalCancellation(_skipCTS.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                EndWindow();
            }
        }

        private async UniTask TransitionToCommentPage()
        {
            commentPageGO.SetActive(true);
            commentPageRect.anchoredPosition = new Vector2(100f, 0);
            commentPageCanvasGroup.alpha = 0f;
            commentPageCanvasGroup.blocksRaycasts = false;

            _currentSequence?.Kill();
            _currentSequence = DOTween.Sequence();

            _currentSequence.Append(statPageCanvasGroup.DOFade(0f, fadeOutDuration).SetEase(fadeEase));
            _currentSequence.Join(statPageRect.DOAnchorPosX(-100f, fadeOutDuration).SetEase(fadeEase));

            _currentSequence.AppendCallback(() =>
            {
                statPage.SetActive(false);
                statPageCanvasGroup.blocksRaycasts = false;
                commentPageRect.anchoredPosition = new Vector2(100f, 0);
            });

            _currentSequence.Append(commentPageCanvasGroup.DOFade(1f, fadeInDuration).SetEase(fadeEase));
            _currentSequence.Join(commentPageRect.DOAnchorPosX(0f, fadeInDuration).SetEase(fadeEase));

            _currentSequence.AppendCallback(() =>
            {
                commentPageCanvasGroup.blocksRaycasts = true;
            });

            var tcs = new UniTaskCompletionSource();
            _currentSequence.OnComplete(() => tcs.TrySetResult());
            _currentSequence.Play();

            await tcs.Task;
        }

        private void EndWindow()
        {
            _currentSequence?.Kill();
            
            statUI.ClearStats();
            
            statPage.SetActive(false);
            commentPageGO.SetActive(false);
            statPageCanvasGroup.blocksRaycasts = false;
            commentPageCanvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            
            _isPlaying = false;
        }

        private void OnDestroy()
        {
            _skipCTS?.Cancel();
            _skipCTS?.Dispose();
            _currentSequence?.Kill();
        }
    }
}