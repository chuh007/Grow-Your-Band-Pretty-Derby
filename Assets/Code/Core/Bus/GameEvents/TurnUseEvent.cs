namespace Code.Core.Bus.GameEvents
{
    public struct TurnUseEvent : IEvent
    {
        public int Value;

        public TurnUseEvent(int value)
        {
            Value = value;
        }
    }
}