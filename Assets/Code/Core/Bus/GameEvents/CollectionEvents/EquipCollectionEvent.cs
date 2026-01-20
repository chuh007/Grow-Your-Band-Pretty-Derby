using Code.SubSystem.Collection;

namespace Code.Core.Bus.GameEvents.CollectionEvents
{
    public struct EquipCollectionEvent : IEvent
    {
        public CollectionDataSO Collection;

        public EquipCollectionEvent(CollectionDataSO collection)
        {
            Collection = collection;
        }
    }
}