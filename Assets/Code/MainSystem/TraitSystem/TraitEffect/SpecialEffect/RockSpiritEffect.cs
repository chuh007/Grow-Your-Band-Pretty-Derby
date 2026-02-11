using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 록스피릿 효과
    /// </summary>
    public class RockSpiritEffect : MultiStatModifierEffect, ITrainingSuccessBonus
    {
        public float AddValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AddValue = GetValue(0);
        }
        
        public void OnTrainingSuccess(MemberType member)
        {
            //StatManager.Instance.IncreaseCondition(member, AddValue);
        }
        
        // TODO : StatManager 에 해당 메서드 구현 필요
        // public void IncreaseCondition(MemberType member, float amount)
        // {
        //     // if (_registry.TryGetMember(member, out var memberStat))
        //     // {
        //     //     var unit = memberStat.UnitData;
        //     //     unit.currentCondition = Mathf.Clamp(unit.currentCondition + amount, 0, unit.maxCondition);
        //     // }
        // }
    }
}