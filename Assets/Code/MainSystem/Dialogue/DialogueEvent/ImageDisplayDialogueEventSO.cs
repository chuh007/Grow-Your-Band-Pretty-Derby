using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    [CreateAssetMenu(fileName = "ImageDisplayDialogueEvent", menuName = "SO/Dialogue/Events/ImageDisplayEvent", order = 1)]
    public class ImageDisplayDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private GameObject imagePrefab;

        public override void RaiseDialogueEvent()
        {
            if (imagePrefab != null)
            {
                Bus<ImageDisplayEvent>.Raise(new ImageDisplayEvent(imagePrefab));
            }
            else
            {
                Debug.LogWarning("ImageDisplayDialogueEventSO: imagePrefab is null. No image will be displayed.");
            }
        }
    }
}