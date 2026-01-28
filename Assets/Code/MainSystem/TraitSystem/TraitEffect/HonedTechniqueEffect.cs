using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 단련기술 특성
    /// </summary>
    public class HonedTechniqueEffect : AbstractTraitEffect, IPercentageModifier<ITrainingStat>
    {
        public float Percentage { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Percentage = N1(trait);
        }
    }
}