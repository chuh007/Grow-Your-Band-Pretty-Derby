using Code.MainSystem.Outing;

namespace Code.Core.Bus.GameEvents.OutingEvents
{
    public struct AddOutingEvent : IEvent
    {
        public OutingEvent Event;

        public AddOutingEvent(OutingEvent evt)
        {
            Event = evt;
        }
    }
}