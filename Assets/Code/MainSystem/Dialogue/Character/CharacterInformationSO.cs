using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Member.LS.Code.Dialogue.Character
{
    [Serializable]
    public sealed class CharacterImageDict : SerializedDictionary<CharacterEmotionType, Sprite>
    {
    }

    
    [CreateAssetMenu(fileName = "Character Information", menuName = "SO/Dialogue/Character/Inform", order = 0)]
    public class CharacterInformationSO : ScriptableObject
    {
        [field: SerializeField] public string CharacterName { get; private set; }
        [field: SerializeField] public CharacterImageDict CharacterEmotions { get; private set; }
    }
}