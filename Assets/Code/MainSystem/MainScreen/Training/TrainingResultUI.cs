using System;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingResultUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image trainingResultImage;
        [SerializeField] private List<StatBox> statBoxes;
        [SerializeField] private Button clickCatcher;

        private void Awake()
        {
            clickCatcher.onClick.AddListener(Close);
        }

        private void Close()
        {
            _onClose?.Invoke();
            Destroy(gameObject);
        }


        private Action _onClose;

        public async UniTask Play(Sprite idleSprite, Sprite resultSprite, List<(Sprite icon, int baseValue, int delta)> statData, bool isSuccess, Action onClose)
        {
            _onClose = onClose;
            
            trainingResultImage.sprite = idleSprite;
            await UniTask.Delay(1000);
            trainingResultImage.sprite = resultSprite;
            if (isSuccess)
            {
                for (int i = 0; i < statBoxes.Count; i++)
                {
                    if (i >= statData.Count) break;

                    var (icon, baseVal, delta) = statData[i];
                    statBoxes[i].Set(icon, baseVal, delta);
                }
            }
        }
    }
}