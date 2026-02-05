using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;

namespace Code.MainSystem.StatSystem.BaseStats
{
    public abstract class AbstractStats : MonoBehaviour
    {
        [SerializeField] private string statDataLabel;
        
        protected readonly Dictionary<StatType, BaseStat> _stats = new();
        protected bool _isInitialized;

        protected virtual void Awake() { }
        
        public virtual async Task InitializeAsync()
        {
            if (_isInitialized)
                return;
        
            Debug.Assert(!string.IsNullOrEmpty(statDataLabel), $"{gameObject.name}: Label이 비어있습니다!");

            try
            {
                List<StatData> dataList = await GameManager.Instance.LoadAllAddressablesAsync<StatData>(statDataLabel);
                foreach (var data in dataList)
                {
                    BaseStat baseStat = new BaseStat(data);
                    await baseStat.InitializeAssetsAsync(data);
                    _stats[data.statType] = baseStat;
                }

                _isInitialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{name}] 초기화 실패: {e.Message}");
            }
        }
        
        public void ApplyStatIncrease(StatType statType, float value)
        {
            BaseStat stat = _stats.GetValueOrDefault(statType);

            stat?.PlusValue((int)value);
        }

        public BaseStat GetStat(StatType statType)
        {
            return _stats.GetValueOrDefault(statType);
        }
    }
}