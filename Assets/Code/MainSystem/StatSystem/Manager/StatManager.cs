using UnityEngine;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.MemberStats;
using Code.MainSystem.StatSystem.TeamStats;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.Manager
{
    public enum MemberType
    {
        Guitar, Drums, Bass, Vocal, Piano, Team
    }
    
    public class StatManager : MonoBehaviour
    {
        [SerializeField] private List<MemberStat> memberStats;
        [SerializeField] private TeamStat teamStat;

        private Dictionary<MemberType, MemberStat> _memberMap;

        private void Awake()
        {
            _memberMap = new Dictionary<MemberType, MemberStat>();
            foreach (var member in memberStats)
            {
                _memberMap[member.MemberType] = member;
            }

            Bus<PracticenEvent>.OnEvent += HandlePractice;
        }

        private void HandlePractice(PracticenEvent evt)
        {
            if (evt.Type == PracticenType.Team)
            {
                UpgradeTeamStat(evt.statType, evt.SuccessRate,evt.Value);
            }
            else
            {
                if (evt.statType is StatType.Condition or StatType.Mental)
                {
                    UpgradeCommonStat(evt.memberType, evt.statType, evt.SuccessRate, evt.Value);
                }
                else
                {
                    UpgradeMemberStat(evt.memberType, evt.statType, evt.SuccessRate, evt.Value);
                }
            }
        }
        
        private void OnDestroy()
        {
            Bus<PracticenEvent>.OnEvent -= HandlePractice;
        }

        #region GetStat

        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            return member != null ? member.GetStat(statType) : null;
        }

        public BaseStat GetTeamStat(StatType statType)
        {
            return teamStat != null ? teamStat.GetTeamStat(statType) : null;
        }
        
        public IReadOnlyDictionary<MemberType, BaseStat> GetAllMemberStat(StatType statType)
        {
            Dictionary<MemberType, BaseStat> result = new();

            foreach (var pair in _memberMap)
            {
                BaseStat stat = pair.Value.GetStat(statType);
                if (stat != null)
                    result.Add(pair.Key, stat);
            }

            return result;
        }

        #endregion
        
        #region UpgradeStat

        private void UpgradeMemberStat(MemberType memberType, StatType statType, float successRate, float value)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            member?.StatUpgrade(statType, successRate, value);
        }

        private void UpgradeCommonStat(MemberType memberType, StatType statType, float successRate, float value)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            member?.StatUpgrade(statType, successRate, value);
        }

        private void UpgradeAllMemberStat(StatType statType, float successRate, float value)
        {
            foreach (var member in memberStats)
            {
                member.StatUpgrade(statType, successRate,value);
            }
        }

        private void UpgradeTeamStat(StatType statType, float successRate,float value)
        {
            teamStat.TeamStatUpgrade(statType, successRate, value);
        }

        #endregion
    }
}
