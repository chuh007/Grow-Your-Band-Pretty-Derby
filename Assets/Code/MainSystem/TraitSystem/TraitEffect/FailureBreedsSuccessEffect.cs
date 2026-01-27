using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    /// <summary>
    /// 실패는 성공의 어머니 특성
    /// </summary>
    public class FailureBreedsSuccessEffect : AbstractTraitEffect, IInspirationSystem
    {
        public int GainOnFailure { get; private set; }
        public int MaxInspiration { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            GainOnFailure = (int)N1(trait);
            MaxInspiration = (int)N2(trait);
        }
    }
}