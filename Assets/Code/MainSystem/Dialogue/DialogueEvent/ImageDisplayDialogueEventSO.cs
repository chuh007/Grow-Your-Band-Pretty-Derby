using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    [CreateAssetMenu(fileName = "ImageDisplayDialogueEvent", menuName = "SO/Dialogue/Events/ImageDisplayEvent", order = 1)]
    public class ImageDisplayDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private AssetReferenceGameObject imagePrefab;

        public override void RaiseDialogueEvent()
        {
            if (imagePrefab != null && imagePrefab.RuntimeKeyIsValid())
            {
                Bus<ImageDisplayEvent>.Raise(new ImageDisplayEvent(imagePrefab));
            }
            else
            {
                Debug.LogWarning("ImageDisplayDialogueEventSO: imagePrefab is null or invalid. No image will be displayed.");
            }
        }
    }
}