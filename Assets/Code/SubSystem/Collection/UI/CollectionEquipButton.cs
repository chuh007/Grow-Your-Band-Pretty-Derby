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
        [SerializeField] private MemberType memberType;
        
        private CollectionDataSO _collection;
        
        public void SetCollection(CollectionDataSO collection)
        {
            _collection = collection;
            icon.sprite = collection.icon;
            levelText.text = collection.level.ToString();
        }

        public void Click()
        {
            controller.CollectionEquipOpen();
        }
    }
}