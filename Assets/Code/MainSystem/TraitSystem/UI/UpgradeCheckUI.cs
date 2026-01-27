using TMPro;
using UnityEngine;
using Code.Core.Bus;
using UnityEngine.UI;
using Reflex.Attributes;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Manager;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.Turn;

namespace Code.MainSystem.TraitSystem.UI
{
    public class UpgradeCheckUI : TraitPanelBase, IUIElement<ActiveTrait, int>, ITurnEndComponent
    {
        [Inject] private TraitManager _traitManager;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button checkButton;
        [SerializeField] private Toggle toggle;

        public bool IsCheck { get; private set; }

        private void Awake()
        {
            checkButton.onClick.AddListener(OnCheck);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void EnableFor(ActiveTrait trait, int prevLevel)
        {
            iconImage.sprite = trait.Data.TraitIcon;
            nameText.SetText(trait.Data.TraitName);
            descriptionText.SetText($"Lv.{prevLevel} -> Lv.{trait.CurrentLevel}\n특성이 강화되었습니다.");
            Bus<TraitShowResponded>.Raise(new TraitShowResponded(_traitManager.GetHolder(_traitManager.CurrentMember)));
            Show();
        }

        public void Disable()
        {
            Hide();
        }

        public void TurnEnd()
        {
            IsCheck = false;
            toggle.isOn = false;
        }
        
        private void OnToggleValueChanged(bool isOn)
        {
            IsCheck = isOn;
        }
        
        private void OnCheck()
        {
            Disable();
        }
    }
}