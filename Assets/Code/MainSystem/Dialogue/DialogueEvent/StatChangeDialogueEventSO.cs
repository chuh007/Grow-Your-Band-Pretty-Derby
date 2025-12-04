using Code.Core.Bus;
using UnityEngine;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    [CreateAssetMenu(fileName = "StatChangeDialogueEvent", menuName = "SO/Dialogue/Events/StatChangeEvent", order = 0)]
    public class StatChangeDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private int statChangeAmount;
        public override void RaiseDialogueEvent()
        {
            //Bus<StatChangeEvent>.Raise(new StatChangeEvent(statChangeAmount));
        }
    }
}