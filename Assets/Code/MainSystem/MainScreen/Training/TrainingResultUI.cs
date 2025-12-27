using System;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingResultUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image trainingResultImage;
        [SerializeField] private List<StatBox> statBoxes;

        public Action OnClose;

        public async UniTask Play(
            Sprite idleSprite,
            Sprite resultSprite,
            List<(string name, Sprite icon, int baseValue, int delta)> statData,
            bool isSuccess,
            Action onClose)
        {
            
            
            trainingResultImage.sprite = idleSprite;
            
            for (int i = 0; i < statBoxes.Count; i++)
            {
                if (i >= statData.Count) break;

                var (name, icon, baseValue, _) = statData[i];
                statBoxes[i].Set(name, icon, baseValue, 0); 
            }

            await UniTask.Delay(1000);

            trainingResultImage.sprite = resultSprite;
            if (isSuccess)
            {
                for (int i = 0; i < statBoxes.Count; i++)
                {
                    if (i >= statData.Count) break;

                    var (name, icon, baseValue, delta) = statData[i];
                    statBoxes[i].Set(name, icon, baseValue, delta);
                }
            }
            OnClose = onClose;
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