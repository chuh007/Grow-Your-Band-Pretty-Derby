using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    /// <summary>
    /// 현재 진행 중인 다이알로그를 즉시 종료하는 이벤트 SO
    /// </summary>
    [CreateAssetMenu(fileName = "EndDialogueEvent", menuName = "SO/Dialogue/Events/EndDialogueEvent", order = 0)]
    public class EndDialogueEventSO : BaseDialogueEventSO
    {
        public override void RaiseDialogueEvent()
        {
            // DialogueManager는 DialogueSkipEvent를 수신하면 EndDialogue()를 호출하여 세션을 종료합니다.
            Bus<DialogueSkipEvent>.Raise(new DialogueSkipEvent());
        }
    }
}
