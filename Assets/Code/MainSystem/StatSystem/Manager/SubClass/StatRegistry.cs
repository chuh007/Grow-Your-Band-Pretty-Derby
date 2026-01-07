using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Stats;

namespace Code.MainSystem.StatSystem.Manager.SubClass
{
    public class StatRegistry
    {
        private readonly Dictionary<MemberType, MemberStat> _memberMap;
        private readonly TeamStat _teamStat;

        public StatRegistry(List<MemberStat> memberStats, TeamStat teamStat)
        {
            _memberMap = new Dictionary<MemberType, MemberStat>();
            _teamStat = teamStat;
            
            foreach (var member in memberStats.Where(member => member != null))
                _memberMap.TryAdd(member.MemberType, member);
        }

        public async Task InitializeAsync()
        {
            await _teamStat.InitializeAsync();
        }

        public bool TryGetMember(MemberType type, out MemberStat member)
            => _memberMap.TryGetValue(type, out member);

        public TeamStat GetTeamStat() => _teamStat;

        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
            => _memberMap.GetValueOrDefault(memberType)?.GetStat(statType);

        public BaseStat GetTeamStatValue(StatType statType)
            => _teamStat?.GetTeamStat(statType);

        public IEnumerable<MemberStat> GetAllMembers()
            => _memberMap.Values;
    }
}