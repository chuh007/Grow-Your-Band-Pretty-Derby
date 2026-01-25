using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class InjuryEffect : AbstractTraitEffect, IInjuryModifier
    {
        public float TrainingSuccessPenaltyPercent => _trainingSuccessPenaltyPercent;
        public int InjuryDurationTurns => _injuryDurationTurns;

        private float _trainingSuccessPenaltyPercent;
        private int _injuryDurationTurns;
        
        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _trainingSuccessPenaltyPercent = N1(trait);
            _injuryDurationTurns = (int)N2(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _trainingSuccessPenaltyPercent = 1f;
            _injuryDurationTurns = 0;
        }
    }
}