using TMPro;
using UnityEngine;
using Code.Core.Bus;
using UnityEngine.UI;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitBar : MonoBehaviour, IUIElement<ActiveTrait>
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelPointText;
        
        private ActiveTrait _currentItem;

        public void EnableFor(ActiveTrait item)
        {
            _currentItem = item;
            iconImage.sprite = _currentItem.Data.TraitIcon;
            nameText.SetText(_currentItem.Name);
            levelPointText.SetText(_currentItem.Data.Level == -1 ? $"{_currentItem.Point}" : $"{_currentItem.Point} / {_currentItem.CurrentLevel}.Lv");
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
        
        public void Click()
        {
            Bus<TraitShowItem>.Raise(new TraitShowItem(_currentItem));
        }
    }
}