using Code.MainSystem.TraitSystem.Contexts;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class TestEffect : ITraitEffect
    {
        public void Apply(GameContext context)
        {
            Debug.Log("Apply");
        }

        public void Remove(GameContext context)
        {   
            Debug.Log("Remove");
        }
    }
}