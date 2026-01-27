using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 부상 특성
    /// </summary>
    public class InjuryEffect : AbstractTraitEffect, IPercentageModifier, IStackable
    {
        public float Percentage { get; private set; }
        public int StackCount { get; private set; }
        public int IncreaseStack { get; private set; } = 1;
        public int MaxStack { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
            MaxStack = (int)N2(trait);
        }

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.UnregisterModifier(this);
            Percentage = 0f;
            MaxStack = 0;
        }
    }
}