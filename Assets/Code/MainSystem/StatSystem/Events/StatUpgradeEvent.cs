using Code.Core.Bus;

namespace Code.MainSystem.StatSystem.Events
{
    public struct StatUpgradeEvent : IEvent
    {
        public bool Upgrade;
        
        public StatUpgradeEvent(bool upgrade)
        {
            Upgrade = upgrade;
        }
    }
}