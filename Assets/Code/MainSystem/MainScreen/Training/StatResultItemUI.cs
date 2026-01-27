using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatResultItemUI : MonoBehaviour
    {
        [SerializeField] private Image leftIcon;
        [SerializeField] private Image rightIcon;
        [SerializeField] private TextMeshProUGUI statNameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Transform gaugeFill;

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
        }

        public void StopAnimation()
        {
            _gaugeTween?.Kill();
            _valueTween?.Kill();
            _gaugeTween = null;
            _valueTween = null;
        }

        public async UniTask AnimateToValue(Sprite left, float currentValue, float duration = 0.5f, CancellationToken token = default)
        {
            StopAnimation();
            
            if (leftIcon != null)
                leftIcon.sprite = left;

            float fromScale = 0f;
            float toScale = Mathf.Clamp01(currentValue / maxValue);

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
            _valueTween = DOTween.To(() => displayedValue, x =>
                {
                    displayedValue = x;
                    if (valueText != null)
                        valueText.text = $"{x}%";
                }, Mathf.RoundToInt(currentValue), duration)
                .SetEase(Ease.OutCubic);

            var valueTask = _valueTween.AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAny(valueTask, UniTask.WaitUntilCanceled(token));
            
            if (token.IsCancellationRequested)
            {
                _valueTween?.Kill();
                throw new System.OperationCanceledException();
            }
        }
        
        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}