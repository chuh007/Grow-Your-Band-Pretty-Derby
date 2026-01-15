using TMPro;
using UnityEngine;
using Code.Core.Bus;
using UnityEngine.UI;
using Reflex.Attributes;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Manager;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitItem : MonoBehaviour, IUIElement<ActiveTrait>
    {
        [Inject] private TraitManager _traitManager;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI levelPointText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        private ActiveTrait _currentItem;
        
        public void EnableFor(ActiveTrait item)
        {
            _currentItem = item;
            iconImage.sprite = _currentItem.Data.TraitIcon;
            levelPointText.SetText(_currentItem.Data.Level == -1 ? $"{_currentItem.Point}" : $"{_currentItem.Point} / {_currentItem.Data.Level}.Lv");
            descriptionText.SetText(_currentItem.Data.DescriptionEffect);
            gameObject.SetActive(true);
        }

        public void RemoveTrait()
        {
            Bus<TraitRemoveRequested>.Raise(new TraitRemoveRequested(_traitManager.CurrentMember, _currentItem.Type));
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(_traitManager.CurrentMember));
            Disable();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}