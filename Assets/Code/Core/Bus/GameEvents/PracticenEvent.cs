using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents
{
    public struct PracticenEvent : IEvent
    {
        public PracticenType Type;
        public MemberType MemberType;
        public StatType StatType;
        public float SuccessRate;
        public float Value;

        public PracticenEvent(PracticenType type, MemberType memberType,StatType statType,float successRate,float value)
        {
            this.Type = type;
            this.MemberType = memberType;
            this.Value = value;
            this.SuccessRate = successRate;
            this.StatType = statType;
        }
    }
}