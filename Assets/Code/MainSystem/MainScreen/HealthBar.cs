using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class HealthBar : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private RectTransform fillBar;           
        [SerializeField] private RectTransform damagePreviewBar;  
        [SerializeField] private TextMeshProUGUI valueText;

        private float maxHealth;
        private float currentHealth;

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
            
            fillBar.localScale = new Vector3(currentRatio, 1f, 1f);
            
            float damageRatio = currentRatio - previewRatio;
            damagePreviewBar.localScale = new Vector3(damageRatio, 1f, 1f);
            
            float previewPos = -(1 - currentRatio) * fillBar.rect.width;
            damagePreviewBar.anchoredPosition = new Vector2(previewPos, 0);

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