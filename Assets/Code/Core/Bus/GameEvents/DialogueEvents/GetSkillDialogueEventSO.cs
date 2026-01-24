using Code.MainSystem.Dialogue.DialogueEvent;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;
using UnityEngine;

namespace Code.Core.Bus.GameEvents.DialogueEvents
{
    [CreateAssetMenu(fileName = "GetSkillDialogueEvent", menuName = "SO/Dialogue/Events/GetSkillEvent", order = 0)]
    public class GetSkillDialogueEventSO : BaseDialogueEventSO
    {
        [SerializeField] private TraitType traitType;
        
        public override void RaiseDialogueEvent()
        {
            Bus<DialogueGetSkillEvent>.Raise(new DialogueGetSkillEvent(traitType));
        }
    }
}