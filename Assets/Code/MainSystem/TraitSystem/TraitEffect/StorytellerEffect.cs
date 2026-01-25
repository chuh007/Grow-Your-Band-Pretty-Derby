using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class StorytellerEffect : AbstractTraitEffect, IStorytellerModifier
    {
        public float MentalStatBonusValue => _mentalStatBonusValue;

        private float _mentalStatBonusValue;

        public override bool CanApply(ITraitHolder holder, ActiveTrait trait)
        {
            return true;
        }

        protected override void ApplyEffect(ITraitHolder holder, ActiveTrait trait)
        {
            _mentalStatBonusValue = N1(trait);
            (holder as IModifierProvider)?.RegisterModifier(this);
        }

        protected override void RemoveEffect(ITraitHolder holder, ActiveTrait trait)
        {
            (holder as IModifierProvider)?.UnregisterModifier(this);
            _mentalStatBonusValue = 0f;
        }
    }
}