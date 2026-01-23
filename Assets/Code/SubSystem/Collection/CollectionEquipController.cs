using System;
using System.Collections.Generic;
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
        [SerializeField] private EquipCollectionListSO equipCollection;
        [SerializeField] private CellInitializer cellInitializer;
        [SerializeField] private GameObject collectionUI;
        
        private Dictionary<MemberType, CollectionDataSO> _collections;
        
        private void Awake()
        {
            _collections = new Dictionary<MemberType, CollectionDataSO>();
            Bus<EquipCollectionEvent>.OnEvent += HandleEquipEvent;
        }

        private void OnDestroy()
        {
            Bus<EquipCollectionEvent>.OnEvent -= HandleEquipEvent;
        }

        private void HandleEquipEvent(EquipCollectionEvent evt)
        {
            collectionUI.SetActive(false);
        }

        public void SaveEquipCollection()
        {
            _collections.Clear();
            foreach (var collection in _collections.Values)
            {
                equipCollection.collections.Add(collection);
            }
        }

        public void CollectionEquipOpen()
        {
            collectionUI.SetActive(true);
        }
    }
}