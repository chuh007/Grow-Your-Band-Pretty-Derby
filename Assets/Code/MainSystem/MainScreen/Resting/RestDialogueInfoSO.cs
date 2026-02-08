using Code.MainSystem.Dialogue;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.MainScreen.Resting
{
    [CreateAssetMenu(fileName = "Rest dialogue", menuName = "SO/Rest/Rest dialogue info", order = 0)]
    public class RestDialogueInfoSO : ScriptableObject
    {
        public MemberType MemberType;
        public AssetReferenceT<DialogueInformationSO> RestInformation;
    }
}