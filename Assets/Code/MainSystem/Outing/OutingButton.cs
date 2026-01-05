using System;
using Code.Core.Bus;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    // 외출 버튼에 달아놓는 컴포넌트
    [RequireComponent(typeof(SceneLoadButton))]
    public class OutingButton : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;
        [SerializeField] private MainScreen.MainScreen mainScreen;
        
        private SceneLoadButton _loadButton;
        private Button _button;
        
        private void Awake()
        {
            _loadButton = GetComponent<SceneLoadButton>();
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(LoadAndDataSend);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void LoadAndDataSend()
        {

            if (mainScreen.UnitSelector.CurrentUnit == null)
            {
                Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
                return;
            }
            if (TrainingManager.Instance.IsMemberTrained(mainScreen.UnitSelector.CurrentUnit.memberType))
                return;
            resultSender.targetMember = mainScreen.UnitSelector.CurrentUnit;
            _loadButton.SceneLoadAdditive("OutingScene");
        }
    }
}