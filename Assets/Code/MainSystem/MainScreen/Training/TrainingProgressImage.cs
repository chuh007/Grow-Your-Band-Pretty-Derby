using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingProgressImage : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private float pulseScale = 0.95f;
        [SerializeField] private float floatOffsetY = 10f;
        [SerializeField] private float animationDuration = 0.3f;

        private Vector3 originalScale;
        private Vector2 originalPosition;

        private void Awake()
        {
            if (progressImage != null)
            {
                originalScale = progressImage.rectTransform.localScale;
                originalPosition = progressImage.rectTransform.anchoredPosition;
            }
        }

        public void SetProgressImage(Sprite progressImageSprite)
        {
            progressImage.sprite = progressImageSprite;
            PlayPulseFloatAnimation();
        }

        private void PlayPulseFloatAnimation()
        {
            RectTransform rt = progressImage.rectTransform;

            rt.DOKill();
            rt.localScale = originalScale * 1.1f; 
            rt.anchoredPosition = originalPosition;

            float half = animationDuration / 2f;

            Sequence anim = DOTween.Sequence();

            anim.Append(
                rt.DOScale(originalScale * 0.95f, half).SetEase(Ease.InBack) 
            );
            anim.Join(
                rt.DOAnchorPosY(originalPosition.y + floatOffsetY, half).SetEase(Ease.OutSine)
            );

            anim.Append(
                rt.DOScale(originalScale, half).SetEase(Ease.OutBack) 
            );
            anim.Join(
                rt.DOAnchorPosY(originalPosition.y, half).SetEase(Ease.InSine)
            );
        }



        public void StopAnimation()
        {
            RectTransform rt = progressImage.rectTransform;
            rt.DOKill();
            rt.localScale = originalScale;
            rt.anchoredPosition = originalPosition;
        }
    }
}
