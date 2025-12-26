using Code.MainSystem.Dialogue;
using UnityEngine;

namespace Code.Core.Bus.GameEvents
{
    public struct DialogueProgressEvent : IEvent
    {
        public DialogueNode NextDialogueNode { get; }
        public Sprite BackgroundImage { get; }

        public DialogueProgressEvent(DialogueNode nextDialogueNode, Sprite backgroundImage)
        {
            NextDialogueNode = nextDialogueNode;
            BackgroundImage = backgroundImage;
        }
    }
}
