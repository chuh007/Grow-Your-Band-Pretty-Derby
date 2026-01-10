using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamTrainingProgressImage : MonoBehaviour
    {
        [System.Serializable]
        public class MemberImage
        {
            public MemberType memberType;
            public Image image;
        }

        [SerializeField] private List<MemberImage> memberImages;
        [SerializeField] private float pulseScale = 0.95f;
        [SerializeField] private float floatOffsetY = 10f;
        [SerializeField] private float animationDuration = 0.3f;

        private readonly Dictionary<MemberType, (RectTransform rt, Vector3 originalScale, Vector2 originalPos)> _imageData = new();

        private void Awake()
        {
            foreach (var m in memberImages)
            {
                if (m.image != null)
                {
                    var rt = m.image.rectTransform;
                    _imageData[m.memberType] = (rt, rt.localScale, rt.anchoredPosition);
                    m.image.gameObject.SetActive(false); 
                }
            }
        }

        public void SetProgressImages(Sprite sprite, HashSet<MemberType> activeMembers)
        {
            foreach (var m in memberImages)
            {
                bool isActive = activeMembers.Contains(m.memberType);
                m.image.gameObject.SetActive(isActive);
                if (isActive)
                {
                    m.image.sprite = sprite;
                    PlayPulseFloatAnimation(m.memberType);
                }
            }
        }

        private void PlayPulseFloatAnimation(MemberType memberType)
        {
            if (!_imageData.TryGetValue(memberType, out var data)) return;

            var rt = data.rt;
            rt.DOKill();
            rt.localScale = data.originalScale * 1.1f;
            rt.anchoredPosition = data.originalPos;

            float half = animationDuration / 2f;
            float delay = 0.05f;

            Sequence anim = DOTween.Sequence();

            anim.AppendInterval(delay);
            anim.Append(rt.DOScale(data.originalScale * 0.9f, half).SetEase(Ease.InBack));
            anim.AppendInterval(delay);
            anim.Append(rt.DOAnchorPosY(data.originalPos.y + floatOffsetY * 1.5f, half).SetEase(Ease.OutSine));
            anim.Append(rt.DOScale(data.originalScale, half).SetEase(Ease.OutElastic));
            anim.Join(rt.DOAnchorPosY(data.originalPos.y, half).SetEase(Ease.InSine));
            anim.AppendInterval(0.8f);
            anim.Append(rt.DOScale(1.1f, 1.15f).SetEase(Ease.Linear));
        }

        public void StopAllAnimations()
        {
            foreach (var kvp in _imageData)
            {
                var rt = kvp.Value.rt;
                rt.DOKill();
                rt.localScale = kvp.Value.originalScale;
                rt.anchoredPosition = kvp.Value.originalPos;
            }
        }
    }
}
