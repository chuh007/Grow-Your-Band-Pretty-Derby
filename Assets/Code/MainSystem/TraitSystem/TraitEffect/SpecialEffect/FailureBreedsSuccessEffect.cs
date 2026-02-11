using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 실패는 성공의 어머니 효과
    /// </summary>
    public class FailureBreedsSuccessEffect : MultiStatModifierEffect, IInspirationSystem, ISuccessGuarantor
    {
        private float _currentInspiration;
        public override bool IsTargetStat(TraitTarget category) => false;
        public override float GetAmount(TraitTarget category, object context = null) => 0;

        public void OnTrainingFailed() 
            => _currentInspiration += GetValue(0);
        
        public bool ShouldGuarantee()
            => _currentInspiration >= GetValue(1);
    }
}