using UnityEngine;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    public abstract class BaseDialogueEventSO : ScriptableObject
    {
        public abstract void RaiseDialogueEvent();
    }
}