using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitTrigger
    {
        void ExecuteTrigger(TraitTrigger trigger, object context = null);
        float QueryTriggerValue(TraitTrigger trigger, object context = null);
        bool CheckTriggerCondition(TraitTrigger trigger, object context = null);
    }
}