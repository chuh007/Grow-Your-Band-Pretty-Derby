using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Dialogue.DialogueEvent
{
    [CreateAssetMenu(fileName = "StatChangeDialogueEvent", menuName = "SO/Dialogue/Events/StatChangeEvent", order = 0)]
    public class StatChangeDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private StatVariation stat;
        
        public override void RaiseDialogueEvent()
        {
            Bus<DialogueStatUpgradeEvent>.Raise(new DialogueStatUpgradeEvent(stat));
        }
    }
}