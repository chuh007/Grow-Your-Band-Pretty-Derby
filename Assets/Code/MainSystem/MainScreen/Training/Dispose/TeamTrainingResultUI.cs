using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamTrainingResultUI : MonoBehaviour, IPointerClickHandler
    {
        [Serializable]
        public class MemberImage
        {
            public MemberType memberType;
            public Image image;
        }

        [SerializeField] private List<MemberImage> trainingResultImages;
        [SerializeField] private Image resultImage;
        [SerializeField] private StatBox teamStatBox;
        [SerializeField] private float scaleFactor = 1.05f;
        [SerializeField] private float scaleTime = 0.5f;

        private readonly Dictionary<MemberType, Image> memberImageDict = new();
        private Action onClose;

        private void Awake()
        {
            foreach (var m in trainingResultImages)
            {
                memberImageDict[m.memberType] = m.image;
                m.image.gameObject.SetActive(false);
            }
        }

        public async UniTask PlayTeamResult(
            Sprite resultSprite,
            Dictionary<MemberType, Sprite> memberResultSprites,
            (string name, Sprite icon, int baseValue, int delta) teamStat,
            bool isSuccess,
            Action onClose)
        {
            this.onClose = onClose;

            foreach (var kvp in memberImageDict)
            {
                kvp.Value.gameObject.SetActive(false);
            }

            teamStatBox.Set(
                teamStat.name,
                teamStat.icon,
                teamStat.baseValue,
                isSuccess ? teamStat.delta : 0);

            await UniTask.Delay(1000);

            resultImage.sprite = resultSprite;
            resultImage.transform.localScale = Vector3.zero;
            resultImage.gameObject.SetActive(true);

            foreach (var kvp in memberImageDict)
            {
                if (memberResultSprites.TryGetValue(kvp.Key, out var sprite))
                {
                    kvp.Value.sprite = sprite;
                    kvp.Value.gameObject.SetActive(true);
                }
                else
                {
                    kvp.Value.gameObject.SetActive(false);
                }
            }

            await resultImage.transform
                .DOScale(Vector3.one * scaleFactor, scaleTime)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            onClose?.Invoke();
        }
    }
}
