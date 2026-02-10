using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Manager;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;
using Reflex.Attributes;
using TMPro;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitTest : MonoBehaviour
    {
        [SerializeField] private TraitDataSO traitData;
        [SerializeField] private TextMeshProUGUI label;
        [Inject] TraitManager _manager;
        [SerializeField] private MemberType memberType;
        [SerializeField] private TraitType traitType;
        
        public void ShowList()
        {
            label.SetText($"{memberType} 특성 UI");
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(memberType));
        }

        public void AddTrait()
        {
            Bus<TraitAddRequested>.Raise(new TraitAddRequested(_manager.CurrentMember, traitData.TraitID));
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(_manager.CurrentMember));
        }
    }
}