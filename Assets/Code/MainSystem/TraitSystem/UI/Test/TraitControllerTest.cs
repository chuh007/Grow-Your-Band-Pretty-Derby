using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.UI.Test
{
    public class TraitControllerTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private MemberType memberType;
        
        public void ShowList()
        {
            Debug.Log(memberType.ToString());
            label.SetText($"{memberType.ToString()}의 특성 패널");
            Bus<TraitShowRequested>.Raise(new TraitShowRequested(memberType));
        }
    }
}