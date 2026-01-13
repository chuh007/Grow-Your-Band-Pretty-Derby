using System.Collections;
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
        [SerializeField] private float imageDuration = 0.15f;

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
            StopAllCoroutines();
            foreach (var m in memberImages)
            {
                m.image.gameObject.SetActive(false);
            }
            StartCoroutine(PlayMembersSequentially(sprite, activeMembers));
        }

        private IEnumerator PlayMembersSequentially(Sprite sprite, HashSet<MemberType> activeMembers)
        {
            foreach (var m in memberImages)
            {
                bool isActive = activeMembers.Contains(m.memberType);
                if (!isActive) continue;

                m.image.sprite = sprite;

                var image = m.image;
                var rt = image.rectTransform;

                image.gameObject.SetActive(true);

                rt.localScale = Vector3.zero;
                var color = image.color;
                color.a = 0f;
                image.color = color;

                float appearDuration = 0.15f;

                Tween scaleTween = rt.DOScale(_imageData[m.memberType].originalScale, appearDuration).SetEase(Ease.OutBack);
                Tween fadeTween = image.DOFade(1f, appearDuration);

                yield return DOTween.Sequence().Join(scaleTween).Join(fadeTween).WaitForCompletion();

                yield return new WaitForSeconds(imageDuration);
            }
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
