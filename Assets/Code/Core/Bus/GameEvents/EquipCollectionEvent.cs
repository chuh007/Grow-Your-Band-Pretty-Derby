using Code.SubSystem.Collection;

namespace Code.Core.Bus.GameEvents
{
    public struct EquipCollectionEvent : IEvent
    {
        public CollectionDataSO CollectionData;

        public EquipCollectionEvent(CollectionDataSO collectionData)
        {
            CollectionData = collectionData;
        }
    }
}