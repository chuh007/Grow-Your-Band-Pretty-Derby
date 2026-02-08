using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatResultItemUI : MonoBehaviour
    {
        [SerializeField] private Image leftIcon;
        [SerializeField] private Image rightIcon;
        [SerializeField] private TextMeshProUGUI statNameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Transform gaugeFill;

        [Header("Rank Progress")]
        [SerializeField] private TextMeshProUGUI nextRankText; 

        private float maxValue = 100f;
        private Tween _gaugeTween;
        private Tween _valueTween;

        public void SetInitialData(string statName, Sprite right)
        {
            statNameText.text = statName;

            if (rightIcon != null)
                rightIcon.sprite = right;

            if (valueText != null)
                valueText.text = "";
            
            if (nextRankText != null)
                nextRankText.text = "";
        }
        
        public void ResetUI()
        {
            StopAnimation();
            
            if (statNameText != null)
                statNameText.text = "";

            if (valueText != null)
                valueText.text = "";
            
            if (leftIcon != null)
                leftIcon.sprite = null;

            if (rightIcon != null)
                rightIcon.sprite = null;
            
            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(0f, 1f, 1f);
            
            if (nextRankText != null)
                nextRankText.text = "";
        }

        public void StopAnimation()
        {
            _gaugeTween?.Kill();
            _valueTween?.Kill();
            _gaugeTween = null;
            _valueTween = null;
        }
        
        public void ForceSetValue(Sprite left, float currentValue, BaseStat baseStat = null)
        {
            StopAnimation();
            
            if (leftIcon != null)
                leftIcon.sprite = left;
            
            float toScale = CalculateGaugeProgress(currentValue, baseStat);
            
            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(toScale, 1f, 1f);
            
            if (valueText != null)
            {
                if (baseStat == null)
                {
                    valueText.text = $"{Mathf.RoundToInt(currentValue)}%";
                }
                else
                {
                    valueText.text = $"{Mathf.RoundToInt(currentValue)}";
                }
            }
            
            if (baseStat != null && nextRankText != null)
            {
                UpdateNextRankText(baseStat);
            }
            else if (nextRankText != null)
            {
                nextRankText.text = "";
            }
        }

        public async UniTask AnimateToValue(Sprite left, float currentValue, float duration = 0.5f, CancellationToken token = default, BaseStat baseStat = null)
        {
            StopAnimation();
            
            if (leftIcon != null)
                leftIcon.sprite = left;

            float fromScale = 0f;
            float toScale = CalculateGaugeProgress(currentValue, baseStat);

            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(fromScale, 1f, 1f);

            _gaugeTween = DOTween.To(
                () => fromScale,
                x =>
                {
                    fromScale = x;
                    if (gaugeFill != null)
                        gaugeFill.localScale = new Vector3(x, 1f, 1f);
                },
                toScale,
                duration
            ).SetEase(Ease.OutCubic);

            var gaugeTask = _gaugeTween.AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAny(gaugeTask, UniTask.WaitUntilCanceled(token));
            
            if (token.IsCancellationRequested)
            {
                _gaugeTween?.Kill();
                throw new System.OperationCanceledException();
            }

            int displayedValue = 0;
            
            bool isCondition = (baseStat == null);
            
            _valueTween = DOTween.To(() => displayedValue, x =>
                {
                    displayedValue = x;
                    if (valueText != null)
                    {
                        if (isCondition)
                        {
                            valueText.text = $"{x}%";
                        }
                        else
                        {
                            valueText.text = $"{x}";
                        }
                    }
                }, Mathf.RoundToInt(currentValue), duration)
                .SetEase(Ease.OutCubic);

            var valueTask = _valueTween.AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAny(valueTask, UniTask.WaitUntilCanceled(token));
            
            if (token.IsCancellationRequested)
            {
                _valueTween?.Kill();
                throw new System.OperationCanceledException();
            }
            
            if (baseStat != null && nextRankText != null)
            {
                UpdateNextRankText(baseStat);
            }
            else if (nextRankText != null)
            {
                nextRankText.text = "";
            }
        }
        
        /// <summary>
        /// 게이지 바의 진행도를 계산합니다.
        /// BaseStat이 있으면 현재 등급 구간 내 진행도(0~1), 없으면 전체 0-100 기준 진행도
        /// </summary>
        private float CalculateGaugeProgress(float currentValue, BaseStat baseStat)
        {
            if (baseStat == null || baseStat.RankTable == null)
            {
                return Mathf.Clamp01(currentValue / maxValue);
            }
            
            var rankTable = baseStat.RankTable;
            int intValue = Mathf.RoundToInt(currentValue);
            
            var nextRankThreshold = rankTable.GetNextRankThreshold(intValue);
            var currentRankThreshold = rankTable.GetCurrentRankThreshold(intValue);
            
            if (nextRankThreshold.HasValue && currentRankThreshold.HasValue)
            {
                float rangeSize = nextRankThreshold.Value - currentRankThreshold.Value;
                float progress = (intValue - currentRankThreshold.Value) / rangeSize;
                return Mathf.Clamp01(progress);
            }
            
            if (!nextRankThreshold.HasValue && currentRankThreshold.HasValue)
            {
                return 1f; 
            }
            
            return Mathf.Clamp01(currentValue / maxValue);
        }
        
        private void UpdateNextRankText(BaseStat baseStat)
        {
            if (baseStat == null || baseStat.RankTable == null)
            {
                if (nextRankText != null)
                    nextRankText.text = "";
                return;
            }
            
            var rankTable = baseStat.RankTable;
            var currentValue = baseStat.CurrentValue;
            
            var nextRankThreshold = rankTable.GetNextRankThreshold(currentValue);
            
            if (nextRankThreshold.HasValue)
            {
                int remaining = nextRankThreshold.Value - currentValue;
                nextRankText.text = $"다음 렙까지 {remaining}";
                
                Debug.Log($"[StatResultItemUI] {baseStat.StatName}: 현재값={currentValue}, 다음등급={nextRankThreshold.Value}, 남은값={remaining}");
            }
            else
            {
                nextRankText.text = "최고 등급!";
                Debug.Log($"[StatResultItemUI] {baseStat.StatName}: 최고 등급 달성!");
            }
        }
        
        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}