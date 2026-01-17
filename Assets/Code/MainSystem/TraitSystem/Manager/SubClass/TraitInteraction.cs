using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class TraitInteraction : MonoBehaviour
    {
        private readonly List<ITraitInteraction> _interactions = new();
    
        public void RegisterInteraction(ITraitInteraction interaction)
        {
            _interactions.Add(interaction);
        }
    
        public void ProcessAllInteractions(ITraitHolder holder)
        {
            var traits = holder.ActiveTraits.ToList();
        
            for (int i = 0; i < traits.Count; i++)
            {
                for (int j = i + 1; j < traits.Count; j++)
                {
                    foreach (var interaction in _interactions.Where(interaction => interaction.CanInteract(traits[i], traits[j])))
                    {
                        interaction.ProcessInteraction(holder, traits[i], traits[j]);
                    }
                }
            }
        }
    }
}