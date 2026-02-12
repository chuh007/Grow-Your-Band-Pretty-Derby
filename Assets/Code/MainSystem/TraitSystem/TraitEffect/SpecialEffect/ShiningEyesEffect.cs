using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 반짝이는 눈 효과
    /// </summary>
    public class ShiningEyesEffect : MultiStatModifierEffect, IAdditionalActionProvider
    {
        public float AdditionalActionChance { get; private set; }
        
        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AdditionalActionChance = GetValue(2);
        }
    }
}