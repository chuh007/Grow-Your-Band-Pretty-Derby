using System.Collections.Generic;
using Code.MainSystem.TraitSystem.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class TraitSelectCompo : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private List<Button> memberButtons;
        [SerializeField] private GameObject panel;
        [SerializeField] private TraitMainPanelUI traitUI;
        
        private bool _isOpen;

        private void Awake()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            foreach (var btn in memberButtons)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClickMember(btn.name));
            }
        }
        
        private void OnClickMember(string type)
        {
            if (!_isOpen)
                return;

            traitUI.EnableFor(type);
            Close();
        }
        
        public void Show()
        {
            panel.SetActive(true);
            _isOpen = true;
        }

        public void Close()
        {
            panel.SetActive(false);
            _isOpen = false;
        }
    }
}