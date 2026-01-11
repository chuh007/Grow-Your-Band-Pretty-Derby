using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using Reflex.Attributes;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class TraitManager : MonoBehaviour
    {
        [Inject] private ITraitPointCalculator _calculator;
        
        private const int TestValue = 20; // 임시값

        /// <summary>
        /// 신규 특성 습득 시도
        /// </summary>
        public void TryAcquireTrait(ITraitHolder holder, TraitDataSO newTrait)
        {
            int currentTotal = _calculator.CalculateTotalPoint(holder);

            if (currentTotal + newTrait.Point <= TestValue)
            {
                holder.AddTrait(newTrait);
                Debug.Log($"{newTrait.TraitID} 습득 성공!");
            }
            else
            {
                Debug.Log("포인트 한도 초과! 특성 재구성이 필요합니다.");
            }
        }

        /// <summary>
        /// 특성 제거 로직 (기획서 규칙 반영)
        /// </summary>
        public bool RemoveTrait(CharacterTrait character, ActiveTrait targetTrait)
        {
            if (targetTrait.Data.IsRemove) 
            {
                Debug.LogWarning("이 특성은 제거할 수 없습니다!");
                return false;
            }
            
            if (targetTrait.Data.Point < 0)
            {
                Debug.Log("주의: 마이너스 특성을 제거하여 총 포인트가 상승합니다.");
            }

            character.RemoveActiveTrait(targetTrait);
            return true;
        }
    }
}