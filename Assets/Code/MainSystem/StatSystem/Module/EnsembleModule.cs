using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.Module.Data;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.StatSystem.Module
{
    public class EnsembleModule : MonoBehaviour
    {
        [SerializeField] private string upgradeDataLabel;
        
        private UpgradeData _upgradeData;

        public async Task Initialize()
        { 
            var handle = Addressables.LoadAssetAsync<UpgradeData>(upgradeDataLabel);
            await handle.Task;
            _upgradeData = handle.Result;
        }

        /// <summary>
        /// 합주 성공 확률 계산
        /// </summary>
        public float CalculateSuccessRate(List<float> memberConditions)
        {
            if (memberConditions == null || memberConditions.Count == 0)
                return 0f;
            
            var levels = memberConditions.Select(c => (int)GetConditionLevel(c));
            int avgLevel = Mathf.RoundToInt((float)levels.Sum() / memberConditions.Count);
            avgLevel = Mathf.Clamp(avgLevel, 0, 4);
            
            return _upgradeData.conditionSuccessRates[avgLevel];
        }
        
        public float ApplyEnsembleBonus(float baseValue, MemberType memberType)
        {
            var holder = TraitManager.Instance.GetHolder(memberType);
            return holder.GetCalculatedStat(TraitTarget.Ensemble, baseValue);
        }
        
        /// <summary>
        /// 합주 성공 여부 판정
        /// </summary>
        public bool CheckSuccess(List<float> memberConditions)
        {
            float successRate = CalculateSuccessRate(memberConditions);
            return Random.Range(0f, 100f) < successRate;
        }

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
    }
}