using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

namespace Code.MainSystem.MainScreen
{
    
    
    /// <summary>
    /// 컨디션 바임
    /// </summary>
    
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform fillBar;
        [SerializeField] private RectTransform damagePreviewBar;
        [SerializeField] private TextMeshProUGUI valueText;

        private float maxHealth;
        private float currentHealth;

        [Header("Tween Settings")]
        [SerializeField] private float fillTweenDuration = 0.2f;
        [SerializeField] private float previewTweenDuration = 0.25f;

        public void SetHealth(float current, float max)
        {
            currentHealth = current;
            maxHealth = max;

            UpdateUI(0f);
        }

        public void PrevieMinusHealth(float amount)
        {
            UpdateUI(amount);
        }

        public void ApplyHealth(float amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            UpdateUI(0f);
        }

        private void UpdateUI(float previewDamage)
        {
            float currentRatio = currentHealth / maxHealth;
            float previewRatio = Mathf.Clamp01((currentHealth - previewDamage) / maxHealth);
            
            fillBar.DOScaleX(currentRatio, fillTweenDuration).SetEase(Ease.OutCubic);
            
            float damageRatio = currentRatio - previewRatio;
            damagePreviewBar.DOScaleX(damageRatio, previewTweenDuration).SetEase(Ease.OutCubic);

            float previewPos = -(1 - currentRatio) * fillBar.rect.width;
            damagePreviewBar.DOAnchorPosX(previewPos, previewTweenDuration).SetEase(Ease.OutCubic);
            
            if (previewDamage > 0)
            {
                valueText.text = $"{Mathf.FloorToInt(currentHealth)} <color=red>-{Mathf.FloorToInt(previewDamage)}</color> / {Mathf.FloorToInt(maxHealth)}";
            }
            else
            {
                valueText.text = $"{Mathf.FloorToInt(currentHealth)} / {Mathf.FloorToInt(maxHealth)}";
            }
        }
    }
}
