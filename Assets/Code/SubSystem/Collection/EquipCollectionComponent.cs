using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    public class EquipCollectionComponent : MonoBehaviour
    {
        [SerializeField] private EquipCollectionListSO equipCollectionListSO;

        private void Awake()
        {
            foreach (var collection in equipCollectionListSO.collections)
            {
                foreach (var evt in collection.plusEvents)
                {
                    Bus<AddOutingEvent>.Raise(new AddOutingEvent(evt));
                }
            }
        }
    }
}