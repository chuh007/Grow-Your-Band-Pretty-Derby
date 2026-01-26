using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Rhythm.Stage
{
    public class LiveStageController : StageController
    {
        [Header("Live UI")]
        [SerializeField] private Slider voteGauge;
        [SerializeField] private TextMeshProUGUI voteCountText;
        [SerializeField] private Image fillImage;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        private Color _originalColor;
        private bool _hasCapturedOriginalColor = false;

        private void Awake()
        {
            if (fillImage != null)
            {
                _originalColor = fillImage.color;
                _hasCapturedOriginalColor = true;
            }
        }

        protected override void OnProgressUpdated(float progress, float currentScore)
        {
            if (voteGauge != null)
            {
                voteGauge.value = progress;
            }

            if (voteCountText != null)
            {
                voteCountText.text = $"{currentScore:N0}";
            }

            if (fillImage != null && _hasCapturedOriginalColor)
            {
                if (progress >= 0.8f)
                {
                    fillImage.color = highlightColor;
                }
                else
                {
                    fillImage.color = _originalColor;
                }
            }
        }
    }
}
