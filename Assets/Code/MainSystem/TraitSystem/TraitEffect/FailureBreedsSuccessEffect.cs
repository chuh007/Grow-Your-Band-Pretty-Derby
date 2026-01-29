using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class FailureBreedsSuccessEffect : AbstractTraitEffect, IInspirationSystem
    {
        public int GainOnFailure { get; private set; }
        public int MaxInspiration { get; private set; }

        public int CurrentInspiration { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            GainOnFailure = (int)N1(trait);
            MaxInspiration = (int)N2(trait);
            CurrentInspiration = 0;
        }

        public void OnFailure()
        {
            CurrentInspiration = System.Math.Min(
                CurrentInspiration + GainOnFailure,
                MaxInspiration
            );
        }

        public bool ShouldGuaranteeSuccess()
        {
            if (CurrentInspiration < MaxInspiration)
                return false;

            CurrentInspiration = 0;
            return true;
        }
    }
}