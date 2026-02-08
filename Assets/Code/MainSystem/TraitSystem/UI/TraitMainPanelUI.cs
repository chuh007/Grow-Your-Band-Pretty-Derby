using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using TMPro;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitMainPanelUI : TraitPanelBase, IUIElement<MemberType>
    {
        [SerializeField] private TextMeshProUGUI label;

        public void EnableFor(MemberType memberType)
        {
            label.SetText($"{memberType} 특성 UI");
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(memberType));
            
            Show(); 
        }

        public void Disable()
        {
            Hide();
        }

        protected override void Show()
        {
            base.Show();
            Debug.Log("Trait Panel Show!");
        }

        protected override void Hide()
        {
            base.Hide();
            Debug.Log("Trait Panel Hide!");
        }
    }
}