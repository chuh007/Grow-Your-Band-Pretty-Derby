using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.CollectionEvents;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    // 소장품 장착과 해제를 담당
    // 일단 중복장착 불가능하게.
    public class CollectionEquipController : MonoBehaviour
    {
        public int MaxEquipCount { get; private set; } = 5;
        
        [SerializeField] private EquipCollectionListSO equipCollection;
        
        private List<CollectionDataSO> _collections;

        private void Awake()
        {
            _collections = new List<CollectionDataSO>();
            Bus<EquipCollectionEvent>.OnEvent += HandleEquipEvent;
            Bus<UnEquipCollectionEvent>.OnEvent += HandleUnEquipEvent;
        }
        
        private void OnDestroy()
        {
            Bus<EquipCollectionEvent>.OnEvent -= HandleEquipEvent;
            Bus<UnEquipCollectionEvent>.OnEvent -= HandleUnEquipEvent;
        }

        public bool CanEquipCollection(CollectionDataSO collection)
            => MaxEquipCount >= _collections.Count && !_collections.Contains(collection);

        public bool CanUnEquipCollection(CollectionDataSO collection)
            => _collections.Contains(collection);
        
        private void HandleEquipEvent(EquipCollectionEvent evt)
        {
            _collections.Add(evt.Collection);
        }
        
        private void HandleUnEquipEvent(UnEquipCollectionEvent evt)
        {
            _collections.Remove(evt.Collection);
        }
    }
}