using Code.MainSystem.Dialogue;

namespace Code.Core.Bus.GameEvents
{
    public struct DialogueStartEvent : IEvent
    {
        public readonly DialogueInformationSO DialogueSO;
        public readonly string StoryStageId;

        public DialogueStartEvent(DialogueInformationSO dialogueSO, string storyStageId)
        {
            DialogueSO = dialogueSO;
            StoryStageId = storyStageId;
        }
    }
}