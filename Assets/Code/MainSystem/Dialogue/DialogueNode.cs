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
        [field: SerializeField] public List<DialogueChoice> Choices { get; private set; }
    }

    [System.Serializable]
    public struct DialogueChoice
    {
        /// <summary>
        /// 선택지 텍스트
        /// </summary>
        [field: SerializeField] public string ChoiceText { get; private set; }

        /// <summary>
        /// 선택 시 이동할 대사 인덱스 (-1이면 바로 다음)
        /// </summary>
        [field: SerializeField] public int NextNodeIndex { get; private set; }

        /// <summary>
        /// 선택 시 발생할 이벤트 리스트
        /// </summary>
        [field: SerializeField] public List<BaseDialogueEventSO> Events { get; private set; }
    }
}
