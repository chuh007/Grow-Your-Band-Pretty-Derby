using System;
using UnityEngine;

namespace Code.MainSystem.Outing
{
    [RequireComponent(typeof(SceneLoadButton))]
    public class OutingPlaceButton : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO sender;
        [SerializeField] private OutingDataController dataController;
        [SerializeField] private OutingPlace outingPlace;

        
        private SceneLoadButton _loadButton;

        private void Awake()
        {
            _loadButton = GetComponent<SceneLoadButton>();
        }

        public void Click()
        {
            Debug.Log(dataController.GetMemberOutingData(sender.targetMember.memberType, outingPlace));
            sender.selectedEvent = dataController.GetMemberOutingData(sender.targetMember.memberType, outingPlace);
            _loadButton.SceneLoadAdditive("OutingScene");
        }
    }
}