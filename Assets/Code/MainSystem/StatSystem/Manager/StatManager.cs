using System.Collections.Generic;
using UnityEngine;
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

        public void UpgradeMemberStat(MemberType memberType, StatType statType, float failureRate)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            if (member != null)
                member.MemberStatUpgrade(statType, failureRate);
        }

        public void UpgradeAllMemberStat(StatType statType, float failureRate)
        {
            foreach (var member in memberStats)
            {
                member.MemberStatUpgrade(statType, failureRate);
            }
        }

        public void UpgradeTeamStat(StatType statType, float failureRate)
        {
            teamStat.TeamStatUpgrade(statType, failureRate);
        }

        #endregion
    }
}
