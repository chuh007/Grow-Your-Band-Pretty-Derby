using UnityEngine;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.MemberStats;

namespace Code.MainSystem.StatSystem.TeamStats
{
    public class TeamStats : AbstractStats
    {
        [SerializeField] private List<MemberStat> teamMembers;
        
        private BaseStat _harmonyStatCache;

        protected override void Awake()
        {
            base.Awake();
            
            _harmonyStatCache = CommonStats.GetValueOrDefault(StatType.TeamHarmony);
        }
        
        public BaseStat GetHarmonyStat()
        {
            return _harmonyStatCache;
        }
        
        public void RecalculateHarmony()
        {
            int totalCondition = 0;
            int totalMental = 0;
            int validMemberCount = 0;

            foreach (var member in teamMembers)
            {
                if (member == null) continue;

                var condition = member.GetCommonStat(StatType.Condition);
                var mental = member.GetCommonStat(StatType.Mental);

                if (condition != null && mental != null)
                {
                    totalCondition += condition.CurrentValue;
                    totalMental += mental.CurrentValue;
                    validMemberCount++;
                }
            }

            if (validMemberCount > 0 && _harmonyStatCache != null)
            {
                int averageCondition = totalCondition / validMemberCount;
                int averageMental = totalMental / validMemberCount;
                int newHarmony = (averageCondition + averageMental) / 2;
                
                int clampedHarmony = Mathf.Clamp(
                    newHarmony, 
                    _harmonyStatCache.MinValue, 
                    _harmonyStatCache.MaxValue
                );
                
                int difference = clampedHarmony - _harmonyStatCache.CurrentValue;
                if (difference > 0)
                {
                    _harmonyStatCache.PlusValue(difference);
                }
                else if (difference < 0)
                {
                    _harmonyStatCache.SubtractValue(-difference);
                }
            }
        }

        public void TeamHarmonyUpgrade(float failureValue)
        {
            float randValue = Random.Range(0f, 101f);
            if (randValue <= failureValue)
            {
                Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(false));
            }
            else
            {
                if (_harmonyStatCache != null)
                {
                    _harmonyStatCache.PlusValue(100);
                    Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(true));
                }
            }
        }
    }
}