using System;
using System.Collections.Generic;
using Code.MainSystem.MainScreen.Bottom;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamTrainingResultUI : MonoBehaviour,IPointerClickHandler
    {
        [System.Serializable]
        public class MemberImage
        {
            public MemberType memberType;
            public Image image;
        }

        [SerializeField] private List<MemberImage> trainingResultImages;
        [SerializeField] private Image resultImage;
        [SerializeField] private float scaleFactor = 1.05f;
        [SerializeField] private float scaleTime = 0.5f;

        private readonly Dictionary<MemberType, Image> _memberImageDict = new();
        private Action OnClose;

        private void Awake()
        {
            foreach (var m in trainingResultImages)
            {
                if (m.image != null)
                {
                    _memberImageDict[m.memberType] = m.image;
                    m.image.gameObject.SetActive(false);
                }
            }
        }

        public async UniTask PlayTeamResult(
            Sprite resultSprite,
            Dictionary<MemberType, Sprite> memberResultSprites,
            Action onClose)
        {
            foreach (var kvp in _memberImageDict)
            {
                var memberType = kvp.Key;
                var img = kvp.Value;

                if (memberResultSprites.TryGetValue(memberType, out var sprite))
                {
                    img.sprite = sprite;
                    img.gameObject.SetActive(true);
                }
                else
                {
                    img.gameObject.SetActive(false);
                }
            }

            await UniTask.Delay(1000);

            resultImage.sprite = resultSprite;
            resultImage.transform.localScale = Vector3.zero;
            resultImage.gameObject.SetActive(true);
            resultImage.transform.DOScale(Vector3.one * scaleFactor, scaleTime).SetEase(Ease.OutBack);

            this.OnClose = onClose;
        }

        public void Close()
        {
            OnClose?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Close();
        }
    }
}
