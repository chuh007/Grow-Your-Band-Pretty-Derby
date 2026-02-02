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
        private readonly List<MemberStat> _rawMemberList;
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
            var tasks = new List<Task> { _teamStat.InitializeAsync() };
            tasks.AddRange(_memberMap.Values.Select(member => member.InitializeAsync()));

            await Task.WhenAll(tasks);
        }

        public bool TryGetMember(MemberType type, out MemberStat member)
            => _memberMap.TryGetValue(type, out member);

        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
            => _memberMap.GetValueOrDefault(memberType)?.GetStat(statType);

        public BaseStat GetTeamStatValue()
            => _teamStat?.GetTeamStat();
    }
}