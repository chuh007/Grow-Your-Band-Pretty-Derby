using System;
using Code.MainSystem.Dialogue;
using Code.MainSystem.MainScreen.Training;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingPlaceButton : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO sender;
        [SerializeField] private OutingDataController dataController;
        [SerializeField] private OutingSelectUI selectUI;
        [SerializeField] private OutingForceController forceController;
        [SerializeField] private OutingPlace outingPlace;
        
        [SerializeField] private DialogueInformationSO defaultDialogue;
        
        private Outline _outline;
        
        public OutingPlace OutingPlace => outingPlace;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }

        public void Click()
        {
            if(TrainingManager.Instance.IsMemberTrained(sender.targetMember.memberType)) return;
            
            forceController.SetCamera(outingPlace);
            
            var evt = dataController.GetMemberOutingData(sender.targetMember.memberType, outingPlace);
            sender.selectedEvent = evt.dialogue;
                        
            if (evt.dialogue == null)
            {
                sender.selectedEvent = defaultDialogue;
            }
            
            selectUI.gameObject.SetActive(true);
            selectUI.SetData(outingPlace, evt.description);
            
        }

        public void ActiveFocus(bool active)
        {
            _outline.enabled = active;
        }
    }
}