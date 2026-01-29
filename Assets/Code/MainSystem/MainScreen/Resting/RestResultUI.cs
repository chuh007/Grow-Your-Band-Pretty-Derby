using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Resting
{
    public class RestResultUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image restResultImage;
        [SerializeField] private HealthBar healthBar;

        public Action OnClose;
        private bool _canClose = false;

        public async UniTask Play(
            Sprite idleSprite,
            Sprite resultSprite,
            Action onClose,
            float beforeHealth = -1f,
            float maxHealth = -1f)
        {
            _canClose = false;
            OnClose = onClose;

            if (restResultImage != null && idleSprite != null)
                restResultImage.sprite = idleSprite;

            if (healthBar != null && beforeHealth >= 0f && maxHealth > 0f)
            {
                healthBar.gameObject.SetActive(true);
                healthBar.SetHealth(beforeHealth, maxHealth);
            }

            await UniTask.Delay(1000);

            if (restResultImage != null && resultSprite != null)
                restResultImage.sprite = resultSprite;
            
            _canClose = true;
        }

        public void SetHealth(float currentHealth, float maxHealth)
        {
            if (healthBar == null) return;
            healthBar.SetHealth(currentHealth, maxHealth);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canClose) 
                return;

            OnClose?.Invoke();
            Destroy(gameObject);
        }
    }
}