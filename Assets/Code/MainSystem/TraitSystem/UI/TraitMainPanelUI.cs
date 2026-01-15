using Code.Core.Bus;
using Code.MainSystem.MainScreen;
using Code.MainSystem.StatSystem.Events;
using TMPro;
using UnityEngine;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitMainPanelUI : MonoBehaviour, IUIElement<string>
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private SelectRequiredUI selectRequiredUI;
        [SerializeField] private TextMeshProUGUI label;
        
        private bool _isOpen;

        private void Awake()
        {
            Hide();
        }

        public void EnableFor(string memberType)
        {
            if (!_isOpen)
                return;
            
            if (!System.Enum.TryParse(memberType, out MemberType parsed))
                return;
            
            label.SetText($"{parsed} 특성 UI");
            selectRequiredUI.Close();
            Show();
        }

        public void Disable()
        {
            Hide();
        }
        
        public void TraitPanelOpen()
        {
            Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
            _isOpen = true;
        }

        public void TraitPanelClose()
        {
            _isOpen = false;
        }
        
        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}