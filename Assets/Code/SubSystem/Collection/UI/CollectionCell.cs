using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.SubSystem.Collection.UI
{
    public class CollectionCell : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nameText;
        
        [SerializeField] private Button button;
        private CollectionDataSO _collectionData;
        
        private bool _canEquip = true;
        
        private void Awake()
        {
            button = icon.GetComponent<Button>();
        }
        
        public CollectionDataSO GetCollectionData() => _collectionData;
        
        public void SetCollectionData(CollectionDataSO collectionData, bool canEquip = true)
        {
            if (collectionData == null)
            {
                _collectionData = collectionData;
                icon.sprite = null;
                levelText.text = string.Empty;
                nameText.text = string.Empty;
                UpdateEquipState(false);
                return;
            }
            
            _collectionData = collectionData;
            icon.sprite = collectionData.icon;
            levelText.text = collectionData.level.ToString();
            nameText.text = collectionData.collectionName;
            
            UpdateEquipState(canEquip);
        }
        
        public void UpdateEquipState(bool canEquip)
        {
            _canEquip = canEquip;
            button.interactable = _canEquip;
            icon.color = _canEquip ? Color.white : Color.gray;
        }
        
        private void OnEnable()
        {
            UpdateEquipState(_canEquip);
            icon.gameObject.SetActive(true);
        }
        
        public void Equip()
        {
            if (_collectionData == null) return;
            Bus<EquipCollectionEvent>.Raise(new EquipCollectionEvent(_collectionData));
        }
    }
}