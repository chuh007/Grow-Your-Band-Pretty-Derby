using System.Collections.Generic;
using Code.MainSystem.Dialogue.DialogueEvent;
using Member.LS.Code.Dialogue;
using Member.LS.Code.Dialogue.Character;
using UnityEngine;

namespace Code.MainSystem.Dialogue
{
    [System.Serializable]
    public struct DialogueNode
    {
        [field: SerializeField] public CharacterInformationSO CharacterInformSO { get; private set; }
        [field: SerializeField, TextArea] public string DialogueDetail { get; private set; }
        [field: SerializeField] public NameTagPositionType NameTagPosition { get; private set; }
        [field: SerializeField] public int BackgroundIndex { get; private set; }
        [field: SerializeField] public CharacterEmotionType CharacterEmotion { get; private set; }
        [field: SerializeField] public List<BaseDialogueEventSO> Events { get; private set; }
    }
}
