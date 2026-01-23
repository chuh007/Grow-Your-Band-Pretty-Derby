using System;
using Code.MainSystem.Dialogue;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.MainSystem.Outing
{
    [RequireComponent(typeof(SceneLoadButton))]
    public class OutingPlaceButton : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private OutingResultSenderSO sender;
        [SerializeField] private OutingDataController dataController;
        [SerializeField] private OutingForceController forceController;
        [SerializeField] private OutingPlace outingPlace;
        
        [SerializeField] private DialogueInformationSO defaultDialogue;
        
        private SceneLoadButton _loadButton;
        
        private void Awake()
        {
            _loadButton = GetComponent<SceneLoadButton>();
        }

        public void Click()
        {
            var evt = dataController.GetMemberOutingData(sender.targetMember.memberType, outingPlace);
            sender.selectedEvent = evt;
            if (evt == null)
            {
                sender.selectedEvent = defaultDialogue;
            }
            _loadButton.SceneLoadAdditive("OutingScene");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            forceController.SetCamera(outingPlace);
        }
    }
}