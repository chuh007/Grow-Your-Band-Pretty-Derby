using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents
{
    public struct PracticenEvent : IEvent
    {
        public PracticenType Type;
        public MemberType memberType;
        public StatType statType;
        public float SuccessRate;
        public float Value;

        public PracticenEvent(PracticenType type, MemberType memberType,StatType statType,float successRate,float value)
        {
            this.Type = type;
            this.memberType = memberType;
            this.Value = value;
            this.SuccessRate = successRate;
            this.statType = statType;
        }
    }
}