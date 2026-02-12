using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.Core.Bus.GameEvents
{
    public struct PracticeEvent : IEvent
    {
        public PracticenType Type;
        public MemberType MemberType;
        public StatType StatType;
        public bool IsSuccess;
        public float Value;

        public PracticeEvent(PracticenType type, MemberType memberType, StatType statType, bool isSuccess,
            float value)
        {
            this.Type = type;
            this.MemberType = memberType;
            this.Value = value;
            this.IsSuccess = isSuccess;
            this.StatType = statType;
        }
    }
}