using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.MainSystem.StatSystem.Module.Data;
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

            // 각 컨디션을 레벨(0~4)로 변환
            var levels = memberConditions.Select(c => (int)GetConditionLevel(c));
            
            // 평균 레벨 계산 (반올림)
            int avgLevel = Mathf.RoundToInt((float)levels.Sum() / memberConditions.Count);
            avgLevel = Mathf.Clamp(avgLevel, 0, 4);
            
            // 해당 레벨의 성공률 반환
            return _upgradeData.conditionSuccessRates[avgLevel];
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