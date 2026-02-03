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
    }
}