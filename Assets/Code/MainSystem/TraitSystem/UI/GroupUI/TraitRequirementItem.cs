using TMPro;
using UnityEngine;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI.GroupUI
{
    public class TraitRequirementItem : TraitPanelBase, IUIElement<string, bool>
    {
        [SerializeField] private TextMeshProUGUI traitNameText;

        public void EnableFor(string traitName, bool isActive)
        {
            traitNameText.text = traitName;
            traitNameText.alpha = isActive ? 1f : 0.35f;
            panel.SetActive(true);
        }

        public void Disable()
        {
            traitNameText.SetText("");
            traitNameText.alpha = 1f;
            panel.SetActive(false);
        }
    }
}