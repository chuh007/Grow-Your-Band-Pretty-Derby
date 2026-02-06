using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.Core.Load;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    public class EquipCollectionComponent : MonoBehaviour, IAwakeLoadComponent
    {
        [SerializeField] private EquipCollectionListSO equipCollectionListSO;
        
        public void FirstTimeAwake()
        {
            foreach (var collection in equipCollectionListSO.collections)
            {
                foreach (var evt in collection.plusEvents)
                {
                    Bus<AddOutingEvent>.Raise(new AddOutingEvent(evt));
                }
                
                foreach (var encounter in collection.plusEncounters)
                {
                    Bus<EncounterAddEvent>.Raise(new EncounterAddEvent(encounter));
                }
            }
        }
    }
}