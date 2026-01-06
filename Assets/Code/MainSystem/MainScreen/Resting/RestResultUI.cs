using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Resting
{
    public class RestResultUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image restResultImage;
        [SerializeField] private HealthBar healthBar;

        public Action OnClose;
        
        public async UniTask Play(
            Sprite idleSprite,
            Sprite resultSprite,
            Action onClose,
            float beforeHealth = -1f,
            float maxHealth = -1f)
        {
            if (restResultImage != null && idleSprite != null)
                restResultImage.sprite = idleSprite;

            if (healthBar != null && beforeHealth >= 0f && maxHealth > 0f)
            {
                healthBar.gameObject.SetActive(true);
                healthBar.SetHealth(beforeHealth, maxHealth);
            }
            else if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false);
            }

            await UniTask.Delay(1000);

            if (restResultImage != null && resultSprite != null)
                restResultImage.sprite = resultSprite;
            
            OnClose = onClose;
        }

        public void SetHealth(float currentHealth, float maxHealth)
        {
            if (healthBar == null)
                return;

            healthBar.gameObject.SetActive(true);
            healthBar.SetHealth(currentHealth, maxHealth);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClose != null)
            {
                OnClose?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}