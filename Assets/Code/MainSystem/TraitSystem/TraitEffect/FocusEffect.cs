using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class FocusEffect : AbstractTraitEffect, ITrainingFailModifier, IConditionChangeModifier, IJudgeRangeModifier, IMissCorrectionModifier
    {
        private float _failRateDelta;
        private float _conditionMultiplier = 1f;
        private float _judgeRangeMultiplier = 1f;
        private bool _canCorrectMiss;

        public float FailRateDelta => _failRateDelta;
        public float ConditionChangeMultiplier => _conditionMultiplier;
        public float JudgeRangeMultiplier => _judgeRangeMultiplier;
        public bool CanCorrectMiss => _canCorrectMiss;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _failRateDelta = N1(trait);
            _conditionMultiplier = N2(trait);
            _judgeRangeMultiplier = N3(trait);
            _canCorrectMiss = true;

            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);

            _failRateDelta = 0f;
            _conditionMultiplier = 1f;
            _judgeRangeMultiplier = 1f;
            _canCorrectMiss = false;
        }
    }
}