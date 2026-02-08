using Code.Core.Addressable;
using TMPro;
using UnityEngine;
using Code.Core.Bus;
using UnityEngine.UI;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitBar : TraitPanelBase, IUIElement<ActiveTrait>
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelPointText;
        
        private ActiveTrait _currentItem;

        public async void EnableFor(ActiveTrait item)
        {
            _currentItem = item;

            await SetIconSafeAsync(iconImage, item.Data.TraitIcon);
            nameText.SetText(_currentItem.Data.TraitName);
            levelPointText.SetText(_currentItem.Data.MaxLevel == -1
                ? $"{_currentItem.Data.Point}"
                : $"{_currentItem.Data.Point} / {_currentItem.CurrentLevel}.Lv");
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