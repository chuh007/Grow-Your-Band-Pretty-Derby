using TMPro;
using UnityEngine;
using Code.Core.Bus;
using UnityEngine.UI;
using Reflex.Attributes;
using Code.MainSystem.TraitSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitRemoveUI : MonoBehaviour, IUIElement<ActiveTrait, ITraitHolder>
    {
        [Header("Dependencies")]
        [Inject] private TraitManager _traitManager;
        
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private ActiveTrait _currentTrait;
        private ITraitHolder _currentHolder;
        
        private void Awake()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void OnDestroy()
        {
            if (confirmButton != null)
                confirmButton.onClick.RemoveListener(OnConfirmClicked);
            
            if (cancelButton != null)
                cancelButton.onClick.RemoveListener(OnCancelClicked);
        }

        public void EnableFor(ActiveTrait trait, ITraitHolder holder)
        {
            _currentTrait = trait;
            _currentHolder = holder;
            UpdateUI();
            Show();
        }

        public void Disable()
        {
            _currentTrait = null;
            _currentHolder = null;
            ClearUI();
            Hide();
        }

        private void UpdateUI()
        {
            if (_currentTrait?.Data is null || _currentHolder == null)
                return;

            iconImage.sprite = _currentTrait.Data.TraitIcon;
            nameText.SetText($"{_currentTrait.Name}");
            
            int afterPoint = _currentHolder.TotalPoint - _currentTrait.Point;
            string pointInfo = "삭제 후 특성 포인트\n " +
                               $"{_currentHolder.TotalPoint} / {_currentHolder.MaxPoints} ->  {afterPoint} / {_currentHolder.MaxPoints}";
            descriptionText.SetText(pointInfo);
        }

        private void ClearUI()
        {
            if (iconImage != null)
                iconImage.sprite = null;
            
            if (nameText != null)
                nameText.SetText("");
            
            if (descriptionText != null)
                descriptionText.SetText("");
        }

        public void OnConfirmClicked()
        {
            if (_currentTrait == null || _traitManager == null)
                return;

            RemoveTrait();
            Disable();
        }

        public void OnCancelClicked()
        {
            Disable();
        }

        private void RemoveTrait()
        {
            Bus<TraitRemoveRequested>.Raise(new TraitRemoveRequested(_traitManager.CurrentMember, _currentTrait.Type));
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
            OnCancelClicked();
        }
    }
}