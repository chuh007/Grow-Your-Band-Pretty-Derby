using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 훈련 결과 창에서 개별 스탯 항목을 표시하는 UI 컴포넌트
    /// 
    /// 기능:
    /// - 스탯 게이지 바 애니메이션 (0 → 현재값)
    /// - 숫자 카운트업 애니메이션
    /// - 등급(Rank) 진행도 표시
    /// - 다음 등급까지 남은 값 표시
    /// 
    /// 사용처:
    /// - PracticeResultWindow에서 각 스탯별로 생성하여 표시
    /// </summary>
    public class StatResultItemUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Components")]
        [SerializeField] private Image leftIcon;           // 스탯 아이콘 (왼쪽)
        [SerializeField] private TextMeshProUGUI statNameText;  // 스탯 이름
        [SerializeField] private TextMeshProUGUI valueText;     // 현재 수치
        [SerializeField] private Transform gaugeFill;           // 게이지 바 (Scale로 조절)

        [Header("Rank Progress")]
        [SerializeField] private TextMeshProUGUI nextRankText;  // "다음 등급까지 N" 텍스트

        #endregion

        #region Private Fields

        private const float MAX_CONDITION_VALUE = 100f;  // 컨디션의 최대값
        
        private Tween _gaugeTween;
        private Tween _valueTween;

        #endregion

        #region Initialization

        /// <summary>
        /// 스탯 이름만 설정하고 값은 비움
        /// 애니메이션 시작 전 초기화용
        /// </summary>
        public void SetInitialData(string statName)
        {
            statNameText.text = statName;

            if (valueText != null)
                valueText.text = "";
            
            if (nextRankText != null)
                nextRankText.text = "";
        }

        /// <summary>
        /// UI를 완전히 초기화 (재사용 시)
        /// </summary>
        public void ResetUI()
        {
            StopAnimation();
            
            if (statNameText != null)
                statNameText.text = "";

            if (valueText != null)
                valueText.text = "";
            
            if (leftIcon != null)
                leftIcon.sprite = null;
            
            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(0f, 1f, 1f);
            
            if (nextRankText != null)
                nextRankText.text = "";
        }

        #endregion

        #region Animation Control

        /// <summary>
        /// 진행 중인 모든 애니메이션 중지
        /// </summary>
        public void StopAnimation()
        {
            _gaugeTween?.Kill();
            _valueTween?.Kill();
            _gaugeTween = null;
            _valueTween = null;
        }

        #endregion

        #region Value Setting (Instant)

        /// <summary>
        /// 애니메이션 없이 즉시 값 설정
        /// 
        /// baseStat이 null이면 컨디션으로 간주 ("%"와 0-100 기준)
        /// baseStat이 있으면 일반 스탯 (등급 구간 기준)
        /// </summary>
        public void ForceSetValue(Sprite icon, float currentValue, BaseStat baseStat = null)
        {
            StopAnimation();
            
            // 아이콘 설정
            if (leftIcon != null)
                leftIcon.sprite = icon;
            
            // 게이지 바 즉시 설정
            float gaugeProgress = CalculateGaugeProgress(currentValue, baseStat);
            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(gaugeProgress, 1f, 1f);
            
            // 수치 텍스트 설정
            SetValueText(currentValue, baseStat);
            
            // 다음 등급 텍스트 (스탯만 해당)
            UpdateNextRankText(baseStat);
        }

        private void SetValueText(float currentValue, BaseStat baseStat)
        {
            if (valueText == null) return;

            bool isCondition = (baseStat == null);
            int roundedValue = Mathf.RoundToInt(currentValue);
            
            valueText.text = isCondition 
                ? $"{roundedValue}%" 
                : $"{roundedValue}";
        }

        #endregion

        #region Value Animation

        /// <summary>
        /// 값을 0에서 목표값까지 애니메이션
        /// 
        /// 게이지 바와 숫자 카운트업이 동시에 진행됨
        /// CancellationToken으로 중단 가능
        /// </summary>
        public async UniTask AnimateToValue(
            Sprite icon,
            float currentValue,
            float duration = 0.5f,
            CancellationToken token = default,
            BaseStat baseStat = null)
        {
            StopAnimation();
            
            // 아이콘 설정
            if (leftIcon != null)
                leftIcon.sprite = icon;

            // 게이지 애니메이션 시작
            await AnimateGauge(currentValue, duration, token, baseStat);
            
            // 숫자 카운트업 애니메이션
            await AnimateValueText(currentValue, duration, token, baseStat);
            
            // 다음 등급 텍스트 업데이트
            UpdateNextRankText(baseStat);
        }

        /// <summary>
        /// 게이지 바 애니메이션 (Scale.x 조절)
        /// </summary>
        private async UniTask AnimateGauge(
            float currentValue,
            float duration,
            CancellationToken token,
            BaseStat baseStat)
        {
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
        }

        /// <summary>
        /// 수치 카운트업 애니메이션
        /// </summary>
        private async UniTask AnimateValueText(
            float currentValue,
            float duration,
            CancellationToken token,
            BaseStat baseStat)
        {
            int displayedValue = 0;
            bool isCondition = (baseStat == null);
            
            _valueTween = DOTween.To(
                () => displayedValue,
                x =>
                {
                    displayedValue = x;
                    if (valueText != null)
                    {
                        valueText.text = isCondition 
                            ? $"{x}%" 
                            : $"{x}";
                    }
                },
                Mathf.RoundToInt(currentValue),
                duration
            ).SetEase(Ease.OutCubic);

            var valueTask = _valueTween.AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAny(valueTask, UniTask.WaitUntilCanceled(token));
            
            if (token.IsCancellationRequested)
            {
                _valueTween?.Kill();
                throw new System.OperationCanceledException();
            }
        }

        #endregion

        #region Gauge Calculation

        /// <summary>
        /// 게이지 바의 진행도 계산 (0.0 ~ 1.0)
        /// 
        /// BaseStat이 없으면 (= 컨디션):
        ///   - 0~100 범위에서 현재값 비율 계산
        /// 
        /// BaseStat이 있으면 (= 일반 스탯):
        ///   - 현재 등급 구간 내에서의 진행도 계산
        ///   - 예: F등급(0~9) 중 5 → 5/10 = 0.5
        /// </summary>
        private float CalculateGaugeProgress(float currentValue, BaseStat baseStat)
        {
            // 컨디션: 0~100 기준
            if (baseStat == null || baseStat.RankTable == null)
            {
                return Mathf.Clamp01(currentValue / MAX_CONDITION_VALUE);
            }
            
            // 일반 스탯: 등급 구간 기준
            var rankTable = baseStat.RankTable;
            int intValue = Mathf.RoundToInt(currentValue);
            
            var nextThreshold = rankTable.GetNextRankThreshold(intValue);
            var currentThreshold = rankTable.GetCurrentRankThreshold(intValue);
            
            // 등급 구간 내 진행도 계산
            if (nextThreshold.HasValue && currentThreshold.HasValue)
            {
                float rangeSize = nextThreshold.Value - currentThreshold.Value;
                float progress = (intValue - currentThreshold.Value) / rangeSize;
                return Mathf.Clamp01(progress);
            }
            
            // 최고 등급 도달 시 100%
            if (!nextThreshold.HasValue && currentThreshold.HasValue)
            {
                return 1f;
            }
            
            // Fallback
            return Mathf.Clamp01(currentValue / MAX_CONDITION_VALUE);
        }

        #endregion

        #region Next Rank Display

        /// <summary>
        /// "다음 등급까지 N" 텍스트 업데이트
        /// 스탯만 해당 (컨디션은 등급 없음)
        /// </summary>
        private void UpdateNextRankText(BaseStat baseStat)
        {
            if (nextRankText == null) return;

            // 컨디션이거나 Rank 테이블이 없으면 빈 텍스트
            if (baseStat == null || baseStat.RankTable == null)
            {
                nextRankText.text = "";
                return;
            }
            
            var rankTable = baseStat.RankTable;
            var currentValue = baseStat.CurrentValue;
            
            var nextThreshold = rankTable.GetNextRankThreshold(currentValue);
            
            if (nextThreshold.HasValue)
            {
                int remaining = nextThreshold.Value - currentValue;
                nextRankText.text = $"다음 등급까지 {remaining}";
                
                Debug.Log($"[StatResultItemUI] {baseStat.StatName}: 현재={currentValue}, 다음등급={nextThreshold.Value}, 남은값={remaining}");
            }
            else
            {
                // 최고 등급 도달
                nextRankText.text = "최고 등급!";
                Debug.Log($"[StatResultItemUI] {baseStat.StatName}: 최고 등급 달성!");
            }
        }

        #endregion

        #region Unity Lifecycle

        private void OnDestroy()
        {
            StopAnimation();
        }

        #endregion
    }
}