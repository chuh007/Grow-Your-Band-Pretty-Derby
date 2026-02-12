using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 지나친 열정 효과
    /// </summary>
    public class OverzealousEffect : MultiStatModifierEffect, IConditionModifier, IAdditionalActionProvider
    {
        public float ConditionCostMultiplier { get; private set; }
        public float ConditionRecoveryMultiplier { get; private set; }
        public float AdditionalActionChance { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            ConditionCostMultiplier = GetValue(0);
            ConditionRecoveryMultiplier = GetValue(1);
            AdditionalActionChance = GetValue(2);
        }
        
        // private void CheckAdditionalAction(MemberType member) {
        //     var holder = TraitManager.Instance.GetHolder(member);
        //     float chance = holder.GetModifiers<IAdditionalActionProvider>().Sum(m => m.AdditionalActionChance);
        //
        //     // 반짝이는 눈 조건: 컨디션 좋음 이상 체크 로직 추가 필요
        //     if (Random.Range(0f, 100f) < chance) {
        //         // 턴 종료를 방해하거나 다시 행동 기회를 주는 로직 실행
        //         Debug.Log("추가 행동 발동!");
        //     }
        // }
    }
}