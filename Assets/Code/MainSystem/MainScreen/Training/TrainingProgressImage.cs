using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingProgressImage : MonoBehaviour
    {
        [SerializeField] private Image progressImage;

        private Vector3 initialScale;

        private void Awake()
        {
            if (progressImage != null)
            {
                initialScale = progressImage.rectTransform.localScale;
            }
        }

        public void SetProgressImage(Sprite progressImageSprite)
        {
            progressImage.sprite = progressImageSprite;
        }

        public void SetProgress(float progress)
        {
            if (progressImage != null)
            {
                float scaleFactor = Mathf.Lerp(0f, 1f, progress); 
                progressImage.transform.DOScale(scaleFactor, scaleFactor);
            }
        }
    }
}