using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.CutsceneEvents;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.Training;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingSelectUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private DialogCutsceneSenderSO sender;

        [Header("UI")]
        [SerializeField] private Button enterButton;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private SceneLoadButton loadButton;
        
        [SerializeField] private List<OutingPlaceButton> outingPlaceButtons;
        
        private OutingEvent _currentOutingEvent;
        
        private void Awake()
        {
            gameObject.SetActive(false);
            enterButton.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            gameObject.SetActive(false);
            TrainingManager.Instance.
                MarkMemberTrained(MainHelper.Instance.MainScreen.UnitSelector.CurrentUnit.memberType);
            Bus<DialogCutscenePlayEvent>.Raise(new DialogCutscenePlayEvent(_currentOutingEvent.dialogue));
        }

        public void SetData(OutingEvent evt)
        {
            _currentOutingEvent = evt;
            descriptionText.SetText(evt.description);
            foreach (var button in outingPlaceButtons)
            {
                button.ActiveFocus(button.OutingPlace == evt.place);
            }
        }
    }
}