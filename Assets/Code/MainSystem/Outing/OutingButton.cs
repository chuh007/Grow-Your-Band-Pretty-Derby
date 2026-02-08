using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    // 외출 버튼에 달아놓는 컴포넌트
    public class OutingButton : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO sender;
        [SerializeField] private MainScreen.MainScreen mainScreen;
        
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(UnitSelect);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void UnitSelect()
        {
            if (mainScreen.UnitSelector.CurrentUnit == null)
            {
                Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
                return;
            }
            if (TrainingManager.Instance.IsMemberTrained(mainScreen.UnitSelector.CurrentUnit.memberType))
                return;
            
            Bus<OutingUnitSelectEvent>.Raise(new OutingUnitSelectEvent(mainScreen.UnitSelector.CurrentUnit));
        }
    }
}