namespace Code.Core.Bus.GameEvents
{
    public struct PracticenEvent : IEvent
    {
        public PracticenType Type;
        public float Value;

        public PracticenEvent(PracticenType type, float value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}