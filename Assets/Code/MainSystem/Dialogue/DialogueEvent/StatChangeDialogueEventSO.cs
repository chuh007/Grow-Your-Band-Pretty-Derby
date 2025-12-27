using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    [CreateAssetMenu(fileName = "StatChangeDialogueEvent", menuName = "SO/Dialogue/Events/StatChangeEvent", order = 0)]
    public class StatChangeDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private StatType statType;
        [SerializeField] private int statChangeAmount;
        
        public override void RaiseDialogueEvent()
        {
            Bus<DialogueStatUpgradeEvent>.Raise(new DialogueStatUpgradeEvent(statType, statChangeAmount));
        }
    }
}