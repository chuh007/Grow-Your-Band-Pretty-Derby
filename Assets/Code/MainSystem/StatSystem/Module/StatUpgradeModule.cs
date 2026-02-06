using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Code.MainSystem.StatSystem.Module.Data;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.StatSystem.Module
{
    public enum ConditionLevel
    {
        VeryBad = 0,    // 매우 나쁨 (0-20%)
        Bad = 1,        // 나쁨 (20-40%)
        Normal = 2,     // 보통 (40-60%)
        Good = 3,       // 좋음 (60-80%)
        VeryGood = 4    // 매우 좋음 (80-100%)
    }
    
    public class StatUpgradeModule : MonoBehaviour
    {
        [SerializeField] private string upgradeDataLabel;
        
        private float _currentCondition; 
        private UpgradeData _upgradeData;

        public async Task Initialize()
        { 
            var handle  = Addressables.LoadAssetAsync<UpgradeData>(upgradeDataLabel);
            await handle.Task;
            _upgradeData = handle.Result;
        }

        /// <summary>
        /// 현재 컨디션 레벨 반환
        /// </summary>
        private ConditionLevel GetConditionLevel(float condition)
        {
            return condition switch
            {
                < 20f => ConditionLevel.VeryBad,
                < 40f => ConditionLevel.Bad,
                < 60f => ConditionLevel.Normal,
                < 80f => ConditionLevel.Good,
                _ => ConditionLevel.VeryGood
            };
        }
        
        /// <summary>
        /// 특정 홀더(멤버)의 현재 컨디션 기반 최종 성공 확률을 반환
        /// </summary>
        public float GetFinalSuccessRate(float condition, ITraitHolder holder)
        {
            ConditionLevel level = GetConditionLevel(condition); 
            float baseRate = _upgradeData.conditionSuccessRates[(int)level];
            
            return holder.GetCalculatedStat(TraitTarget.SuccessRate, baseRate);
        }
        
        /// <summary>
        /// 컨디션 레벨에 따른 성공 확률 반환
        /// </summary>
        private float GetSuccessRate()
            => _upgradeData.conditionSuccessRates[(int)GetConditionLevel(_currentCondition)];

        /// <summary>
        /// 훈련 성공 여부 판정
        /// </summary>
        public bool CanUpgrade(ITraitHolder holder)
        {
            var guarantors = holder.GetModifiers<ISuccessGuarantor>();

            if (guarantors.Any(g => g.ShouldGuarantee()))
                return true;
            
            
            float baseRate = GetSuccessRate();
            float finalRate = holder.GetCalculatedStat(TraitTarget.SuccessRate, baseRate);
    
            bool isSuccess = Random.Range(0f, 100f) < finalRate;
            
            foreach (var listener in holder.GetModifiers<IInspirationSystem>())
                if (!isSuccess)
                    listener.OnTrainingFailed();
            

            return isSuccess;
        }

        /// <summary>
        /// 컨디션 설정
        /// </summary>
        public void SetCondition(float value)
            => _currentCondition = Mathf.Clamp(value, 0f, 100f);
    }
}