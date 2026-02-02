using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 독선적 효과
    /// </summary>
    public class DogmaticEffect : AbstractTraitEffect, IEnsembleStat, IPercentageModifier<IEnsembleStat>
    {
        public float Percentage { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
        }
    }
}