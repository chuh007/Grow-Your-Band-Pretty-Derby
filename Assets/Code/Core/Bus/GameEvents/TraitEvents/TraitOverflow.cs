namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct TraitOverflow : IEvent
    {
        public int CurrentPoint { get; }
        public int MaxPoint { get; }

        public TraitOverflow(int currentPoint, int maxPoint)
        {
            CurrentPoint = currentPoint;
            MaxPoint = maxPoint;
        }
    }
}