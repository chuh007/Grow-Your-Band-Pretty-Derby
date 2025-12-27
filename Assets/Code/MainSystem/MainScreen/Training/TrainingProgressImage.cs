using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingProgressImage : MonoBehaviour
    {
        [SerializeField] private Image progressImage;

        public void SetProgressImage(Sprite progressImageSprite)
        {
            progressImage.sprite = progressImageSprite;
        }
    }
}