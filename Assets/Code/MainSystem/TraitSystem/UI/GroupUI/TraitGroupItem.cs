using TMPro;
using UnityEngine;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI.GroupUI
{
    public class TraitGroupItem : TraitPanelBase, IUIElement<TraitGroupStatus>
    {
        [SerializeField] private TextMeshProUGUI groupNameText;
        [SerializeField] private TextMeshProUGUI countText;

        [Header("Trait Requirement")]
        [SerializeField] private TraitRequirementItem requirementItemPrefab;
        [SerializeField] private Transform requirementRoot;
        
        private void BuildRequirementList(TraitGroupStatus status)
        {
            foreach (Transform child in requirementRoot)
                Destroy(child.gameObject);

            foreach (var trait in status.RequiredTraits)
            {
                var item = Instantiate(requirementItemPrefab, requirementRoot);
                bool isActive = status.IsTraitContributed(trait);
                item.EnableFor(trait.TraitName, isActive);
            }
        }

        public void EnableFor(TraitGroupStatus status)
        {
            groupNameText.text = status.GroupData.GroupName;
            countText.text = $"{status.CurrentCount} / {status.MaxCount}";

            BuildRequirementList(status);
        }

        public void Disable()
        {
            
        }
    }
}