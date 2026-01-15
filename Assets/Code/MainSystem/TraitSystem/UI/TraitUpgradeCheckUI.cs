using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager;
using Reflex.Attributes;
using TMPro;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitUpgradeCheckUI : MonoBehaviour, IUIElement<ActiveTrait, int>
    {
        [Inject] private TraitManager _traitManager;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button checkButton;

        private void Awake()
        {
            checkButton.onClick.AddListener(OnCheck);
        }

        private void OnCheck()
        {
            Disable();
        }

        public void EnableFor(ActiveTrait trait, int prevLevel)
        {
            iconImage.sprite = trait.Data.TraitIcon;
            nameText.SetText(trait.Name);
            descriptionText.SetText($"Lv.{prevLevel} -> Lv.{trait.CurrentLevel}\n특성이 강화되었습니다.");
            Bus<TraitShowResponded>.Raise(new TraitShowResponded(_traitManager.GetHolder(_traitManager.CurrentMember)));
            Show();
        }

        public void Disable()
        {
            Hide();
        }
        
        private void Show()
        {
            panel.SetActive(true);
        }

        private void Hide()
        {
            panel.SetActive(false);
        }
    }
}