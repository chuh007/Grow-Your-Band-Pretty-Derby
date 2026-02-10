using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 이신 전심 특성
    /// </summary>
    public class TelepathyEffect : MultiStatModifierEffect
    {
        public override float GetAmount(TraitTarget category, object context = null)
        {
            bool hasPartner = true;
            for (int i = 0; i < (int)MemberType.Team; i++)
            {
                if (!TraitManager.Instance.HasTrait((MemberType)i, _ownerTrait.Data.IDHash))
                    continue;
                
                hasPartner = false;
                break;
            }
            
            if (hasPartner) 
                return GetValue(0) * (GetValue(1) * 0.01f);
            
            return GetValue(0);
        }
    }
}