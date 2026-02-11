using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 고위험 루틴 효과
    /// </summary>
    public class HighRiskRoutineEffect : MultiStatModifierEffect, IRoutineModifier
    {
        private float _addValue;
        private int _highRiskStack = 0; 
        private string _lastPracticeId;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            _addValue = GetValue(0);
        }

        public void OnPracticeSuccess() {
            if (_highRiskStack < 7) 
                _highRiskStack++; 
        }
        
        public void OnRest() {
            _highRiskStack = 0;
            _lastPracticeId = null;
        }
        
        public float GetSuccessBonus(string currentActionId) {
            float bonus = (_lastPracticeId == currentActionId) ? _addValue : 0f;
            _lastPracticeId = currentActionId;
            return bonus;
        }
        
        public float GetStatMultiplier() => 1f + (_highRiskStack * 0.1f);
        
        
        // TODO : StatManager 에 해당 메서드 구현 필요
        // public void ModifyCondition(MemberType memberType, float amount)
        // {
        //     if (_registry.TryGetMember(memberType, out var member))
        //     {
        //         // MemberStat 클래스 내부에 UnitDataSO 필드가 있다고 가정 (또는 캐싱된 데이터 사용)
        //         var unit = member.UnitData; 
        //         if (unit != null)
        //         {
        //             unit.currentCondition = Mathf.Clamp(unit.currentCondition + amount, 0, unit.maxCondition);
        //             // UI 갱신이 필요하다면 여기서 이벤트를 발생시키거나 직접 호출
        //         }
        //     }
        // }
    }
}