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

        public void SetInitialData(string statName, Sprite right)
        {
            statNameText.text = statName;

            if (rightIcon != null)
                rightIcon.sprite = right;

            if (valueText != null)
                valueText.text = "";
        }

        public async UniTask AnimateToValue(Sprite left, float currentValue, float duration = 0.5f)
        {
            if (leftIcon != null)
                leftIcon.sprite = left;

            float fromScale = 0f;
            float toScale = Mathf.Clamp01(currentValue / maxValue);

            if (gaugeFill != null)
                gaugeFill.localScale = new Vector3(fromScale, 1f, 1f);

            await DOTween.To(
                () => fromScale,
                x =>
                {
                    fromScale = x;
                    if (gaugeFill != null)
                        gaugeFill.localScale = new Vector3(x, 1f, 1f);
                },
                toScale,
                duration
            ).SetEase(Ease.OutCubic).AsyncWaitForCompletion();

            int displayedValue = 0;
            await DOTween.To(() => displayedValue, x =>
                {
                    displayedValue = x;
                    if (valueText != null)
                        valueText.text = $"{x}%";
                }, Mathf.RoundToInt(currentValue), duration)
                .SetEase(Ease.OutCubic)
                .AsyncWaitForCompletion();
        }
    }
}