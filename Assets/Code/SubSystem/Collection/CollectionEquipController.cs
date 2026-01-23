using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.SubSystem.Collection.UI;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    // 소장품 장착과 해제를 담당
    // 일단 중복장착 불가능하게.
    public class CollectionEquipController : MonoBehaviour
    {
        public int MaxEquipCount { get; private set; } = 5;
        [SerializeField] private CollectionDatabaseSO collectionDatabase;
        [SerializeField] private EquipCollectionListSO equipCollection;
        [SerializeField] private CellInitializer cellInitializer;
        [SerializeField] private GameObject collectionUI;
        
        private Dictionary<MemberType, CollectionDataSO> _collections;
        
        private Action<CollectionDataSO> _current;
        
        private void Awake()
        {
            _collections = new Dictionary<MemberType, CollectionDataSO>();
            Bus<EquipCollectionEvent>.OnEvent += HandleEquipEvent;
        }

        private void OnDestroy()
        {
            Bus<EquipCollectionEvent>.OnEvent -= HandleEquipEvent;
        }

        public void SaveEquipCollection()
        {
            _collections.Clear();
            foreach (var collection in _collections.Values)
            {
                equipCollection.collections.Add(collection);
            }
        }

        public void CollectionEquipOpen(Action<CollectionDataSO> callback)
        {
            collectionUI.SetActive(true);
            _current = callback;
            
        }
        
        private void HandleEquipEvent(EquipCollectionEvent evt)
        {
            collectionUI.SetActive(false);
            equipCollection.collections.Add(evt.CollectionData);
            _current?.Invoke(evt.CollectionData);
        }
    }
}