using System;
using System.Collections.Generic;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.MainScreen.Training;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingSelectUI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private MainScreen.MainScreen mainScreen;
        [SerializeField] private DialogCutsceneSenderSO sender;

        [Header("UI")]
        [SerializeField] private Button enterButton;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private SceneLoadButton loadButton;
        
        [SerializeField] private List<OutingPlaceButton> outingPlaceButtons;
        
        
        private void Awake()
        {
            gameObject.SetActive(false);
            enterButton.onClick.AddListener(HandleClick);
        }

        private void HandleClick()
        {
            gameObject.SetActive(false);
            TrainingManager.Instance.MarkMemberTrained(mainScreen.UnitSelector.CurrentUnit.memberType);
            loadButton.SceneLoadAdditive("DialogCutscene");
        }

        public void SetData(OutingPlace place, string text)
        {
            descriptionText.SetText(text);
            foreach (var button in outingPlaceButtons)
            {
                button.ActiveFocus(button.OutingPlace == place);
            }
        }
    }
}