using Code.Core;
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
    public class UpgradeCheckUI : TraitPanelBase, IUIElement<ActiveTrait, int>
    {
        [Inject] private TraitManager _traitManager;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button checkButton;

        private void Awake()
        {
            checkButton.onClick.AddListener(OnCheck);
        }

        public async void EnableFor(ActiveTrait trait, int prevLevel)
        {
            Sprite sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trait.Data.IconAddress);
            if (sprite is not null && iconImage is not null)
                iconImage.sprite = sprite;

            nameText.SetText(trait.Data.TraitName);
            descriptionText.SetText($"Lv.{prevLevel} -> Lv.{trait.CurrentLevel}\n특성이 강화되었습니다.");
            Bus<TraitShowResponded>.Raise(new TraitShowResponded(_traitManager.GetHolder(_traitManager.CurrentMember)));
            Show();
        }

        public void Disable()
        {
            Hide();
        }
        
        private void OnCheck()
        {
            Disable();
        }
    }
}