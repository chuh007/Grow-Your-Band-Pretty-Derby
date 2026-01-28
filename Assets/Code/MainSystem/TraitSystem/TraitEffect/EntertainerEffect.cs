using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 만담가 특성
    /// </summary>
    public class EntertainerEffect : AbstractTraitEffect, IAdditiveModifier<IMentalStat>
    {
        public float AdditiveValue { get; private set; }
        
        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AdditiveValue = N1(trait);
        }
    }
}