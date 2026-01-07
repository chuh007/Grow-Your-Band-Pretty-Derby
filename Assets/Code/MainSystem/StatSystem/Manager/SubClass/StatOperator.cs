using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.Manager.SubClass
{
    public class StatOperator
    {
        private readonly StatRegistry _registry;

        public StatOperator(StatRegistry registry)
        {
            _registry = registry;
        }

        public void IncreaseMemberStat(MemberType memberType, StatType statType, float value)
        {
            if (_registry.TryGetMember(memberType, out var member))
                member.ApplyStatIncrease(statType, value);
        }

        public void IncreaseAllStatsForMember(MemberType memberType, float value)
        {
            if (_registry.TryGetMember(memberType, out var member))
                member.ApplyAllStatIncrease(value);
        }

        public void IncreaseStatForAllMembers(StatType statType, float value)
        {
            foreach (var member in _registry.GetAllMembers())
                member?.ApplyStatIncrease(statType, value);
        }

        public void IncreaseTeamStat(float value)
            => _registry.GetTeamStat()?.ApplyTeamStatIncrease(value);
    }
}