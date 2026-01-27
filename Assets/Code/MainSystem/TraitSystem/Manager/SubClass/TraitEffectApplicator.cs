using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.TraitEffect;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class TraitEffectApplicator : MonoBehaviour
    {
        public void ApplyEffects(ITraitHolder holder, IEnumerable<ActiveTrait> traits, TraitEffectType timing)
        {
            foreach (var trait in traits)
            {
                if (trait.Data.traitEffectType != timing)
                    continue;

                AbstractTraitEffect effect = trait.TraitEffect;
                if (effect == null)
                    continue;

                if (effect.CanApply(holder, trait))
                    effect.Apply(holder, trait);
            }
        }
    }
}