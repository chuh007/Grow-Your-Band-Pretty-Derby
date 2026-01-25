using System;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.SubSystem.Collection.UI
{
    [RequireComponent(typeof(Button))]
    public class CollectionEquipButton : MonoBehaviour
    {
        [SerializeField] private CollectionEquipController controller;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private MemberType memberType;
        [SerializeField] private int idx;
        
        private CollectionDataSO _collection;
        
        public void Click()
        {
            controller.CollectionEquipOpen(OnCollectionSelected, memberType);
        }

        private void OnCollectionSelected(CollectionDataSO collection)
        {
            _collection = collection;
            icon.sprite = collection.icon;
            levelText.SetText(collection.level.ToString());
            nameText.SetText(collection.name);
        }
    }
}