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
    public class TraitDetailPanel : MonoBehaviour, IUIElement<ActiveTrait>
    {
        [Header("Dependencies")]
        [Inject] private TraitManager _traitManager;
        
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI levelPointText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button removeButton;
        [SerializeField] private GameObject panel;
        
        private ActiveTrait _currentTrait;

        private void Awake()
        {
            if (removeButton != null)
                removeButton.onClick.AddListener(OnRemoveButtonClicked);
        }

        private void OnDestroy()
        {
            if (removeButton != null)
                removeButton.onClick.RemoveListener(OnRemoveButtonClicked);
        }

        public void EnableFor(ActiveTrait trait)
        {
            _currentTrait = trait;
            UpdateUI();
            Show();
        }

        public void Disable()
        {
            Hide();
        }

        private void UpdateUI()
        {
            if (_currentTrait == null || _currentTrait.Data is null)
                return;

            iconImage.sprite = _currentTrait.Data.TraitIcon;
            
            string pointText = _currentTrait.Data.Level == -1 ? "" : $"Lv.{_currentTrait.Data.Level}";
            levelPointText.SetText(pointText);
            
            descriptionText.SetText(_currentTrait.Data.DescriptionEffect);

            if (removeButton is null)
                return;
            
            bool canRemove = _traitManager is not null && 
                             (_traitManager.GetHolder(_traitManager.CurrentMember)?.IsAdjusting ?? false) 
                             || _currentTrait.Data.IsRemovable;
            removeButton.interactable = canRemove;
        }

        private void OnRemoveButtonClicked()
        {
            if (_currentTrait == null || _traitManager == null)
                return;

            RemoveTrait();
            RefreshTraitList();
            Hide();
        }

        private void RemoveTrait()
        {
            Bus<TraitRemoveRequestedUI>.Raise(new TraitRemoveRequestedUI(_currentTrait,
                _traitManager.GetHolder(_traitManager.CurrentMember)));
        }

        private void RefreshTraitList()
        {
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(_traitManager.CurrentMember));
        }

        private void Show()
        {
            if (panel is not null)
                panel.SetActive(true);
            else
                gameObject.SetActive(true);
        }

        private void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            else
                gameObject.SetActive(false);
        }
        
        public void Close()
        {
            Hide();
        }
    }
}