using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 실패는 성공의 어머니 특성
    /// </summary>
    public class FailureBreedsSuccessEffect : AbstractTraitEffect, IAdditiveModifier, IStackable
    {
        public float AdditiveValue { get; private set; } = 100;
        public int StackCount { get; private set; }
        public int IncreaseStack { get; private set; }
        public int MaxStack { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            StackCount = 0;
            IncreaseStack = (int)N1(trait);
            MaxStack = (int)N2(trait);
        }

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return StackCount >= MaxStack;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            holder?.UnregisterModifier(this);
            StackCount = 0;
            MaxStack = 0;
        }
    }
}