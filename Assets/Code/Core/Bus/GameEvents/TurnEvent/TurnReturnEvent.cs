namespace Code.Core.Bus.GameEvents
{
    public struct TurnReturnEvent : IEvent
    {
        public int Value;

        public TurnReturnEvent(int value)
        {
            Value = value;
        }
    }
}