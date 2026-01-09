using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingResultUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image trainingResultImage;
        [SerializeField] private Image resultImage;
        [SerializeField] private List<StatBox> statBoxes;
        [SerializeField] private float scaleFactor = 1.05f;
        [SerializeField] private float scaleTime = 0.5f;
        [SerializeField] private HealthBar healthBar;

        private Action OnClose;

        public async UniTask Play(
            Sprite idleSprite,
            Sprite resultSprite,
            List<(string name, Sprite icon, int baseValue, int delta)> statData,
            bool isSuccess,
            float curCon,
            Action onClose)
        {
            trainingResultImage.sprite = idleSprite;
            healthBar.SetHealth(100,100);

            resultImage.gameObject.SetActive(false); 

            for (int i = 0; i < statBoxes.Count; i++)
            {
                if (i >= statData.Count) break;
                var (name, icon, baseValue, _) = statData[i];
                statBoxes[i].Set(name, icon, baseValue, 0);
            }

            await UniTask.Delay(1000);

            trainingResultImage.sprite = resultSprite;
            trainingResultImage.transform.DOScale(scaleFactor, scaleTime).SetEase(Ease.InSine);
            
            resultImage.sprite = resultSprite;
            resultImage.transform.localScale = Vector3.zero;
            resultImage.gameObject.SetActive(true);
            resultImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
            
            if (isSuccess)
            {
                for (int i = 0; i < statBoxes.Count; i++)
                {
                    if (i >= statData.Count) break;
                    var (name, icon, baseValue, delta) = statData[i];
                    statBoxes[i].Set(name, icon, baseValue, delta);
                }
            }
            
            healthBar.SetHealth(curCon, 100);

            OnClose = onClose;
        }
        
        

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClose?.Invoke();
        }

       
    }
}
