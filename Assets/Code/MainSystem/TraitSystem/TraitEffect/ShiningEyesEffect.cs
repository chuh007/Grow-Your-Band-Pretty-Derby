using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 반짝이는 눈
    /// </summary>
    public class ShiningEyesEffect : AbstractTraitEffect, IActionPointBonus
    {
        public float Chance { get; private set; }
        public int Amount { get; private set; }
        

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            Chance = N1(trait);
            Amount = (int)N2(trait);
        }
    }
}