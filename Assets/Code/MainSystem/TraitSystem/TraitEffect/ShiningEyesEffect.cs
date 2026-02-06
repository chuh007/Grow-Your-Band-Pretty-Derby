using UnityEngine;

namespace Code.MainSystem.TraitSystem.TraitEffect
{
    public class ShiningEyesEffect : MultiStatModifierEffect
    {
        public void OnTrainingComplete(object conditionState)
        {
            bool isConditionHigh = conditionState is >= 4; 
            
            if (isConditionHigh && Random.Range(0f, 100f) <= GetValue(0))
            {
                // TODO: 시스템에 행동권 N2(GetValue(1)) 추가 로직 호출
            }
        }
    }
}