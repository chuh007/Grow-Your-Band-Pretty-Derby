using Code.SubSystem.Collection;

namespace Code.Core.Bus.GameEvents.CollectionEvents
{
    public struct UnEquipCollectionEvent : IEvent
    {
        public CollectionDataSO Collection;

        public UnEquipCollectionEvent(CollectionDataSO collection)
        {
            Collection = collection;
        }
    }
}