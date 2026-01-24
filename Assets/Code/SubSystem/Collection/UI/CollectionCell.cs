using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.SubSystem.Collection.UI
{
    public class CollectionCell : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private Button _button;
        private CollectionDataSO _collectionData;
        
        private void Awake()
        {
            _button = icon.GetComponent<Button>();
        }

        public void SetCollectionData(CollectionDataSO collectionData)
        {
            _collectionData = collectionData;
            icon.gameObject.SetActive(true);
            icon.sprite = collectionData.icon;
            levelText.text = collectionData.level.ToString();
            nameText.text = collectionData.collectionName;
        }

        public void Equip()
        {
            Bus<EquipCollectionEvent>.Raise(new EquipCollectionEvent(_collectionData));
        }
    }
}