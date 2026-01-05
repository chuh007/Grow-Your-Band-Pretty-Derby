using System;
using System.Collections.Generic;
using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.MainScreen;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine.UI;

namespace Code.MainSystem.StatSystem.UI
{
    public class RestSelectCompo : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private List<Button> memberButtons;
        [SerializeField] private SelectRequiredUI selectRequiredUI;
        [SerializeField] private TrainingSequenceController trainingSequenceController;
        [SerializeField] private HealthBar healthBar;

        private Dictionary<MemberType, UnitDataSO> _unitMap;
        private UnitDataSO _selectedUnit;
        private bool _isOpen;

        private void Awake()
        {
            Bus<ConfirmRestEvent>.OnEvent += OnConfirmRest;
        }

        private void OnDestroy()
        {
            Bus<ConfirmRestEvent>.OnEvent -= OnConfirmRest;
        }

        public void CacheUnits(List<UnitDataSO> units)
        {
            _unitMap = new Dictionary<MemberType, UnitDataSO>();

            foreach (var unit in units)
                _unitMap[unit.memberType] = unit;

            InitButtons();
        }

        private void InitButtons()
        {
            foreach (var btn in memberButtons)
            {
                if (!Enum.TryParse(btn.name, out MemberType type))
                    continue;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClickMember(type));
            }
        }

        public void Rest()
        {
            Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
            _isOpen = true;
        }

        private void OnClickMember(MemberType type)
        {
            if (!_isOpen)
                return;

            if (!_unitMap.TryGetValue(type, out var unit))
                return;

            if (TrainingManager.Instance.IsMemberTrained(type))
                return;

            _selectedUnit = unit;
            Confirm();
        }

        private async void Confirm()
        {
            if (_selectedUnit == null)
                return;

            
            selectRequiredUI.Close();
            
            float beforeHealth = _selectedUnit.currentCondition;
            float healAmount = _selectedUnit.maxCondition * 0.3f;
            float afterHealth = Mathf.Min(
                beforeHealth + healAmount,
                _selectedUnit.maxCondition
            );

            _selectedUnit.currentCondition = afterHealth;

            trainingSequenceController.gameObject.SetActive(true);
            await trainingSequenceController.PlayRestSequence(
                _selectedUnit,
                beforeHealth,
                afterHealth
            );
            
            Bus<ConfirmRestEvent>.Raise(new ConfirmRestEvent(_selectedUnit));
            
            healthBar.SetHealth(_selectedUnit.currentCondition, _selectedUnit.maxCondition);

            _isOpen = false;
            _selectedUnit = null;
        }

        private void OnConfirmRest(ConfirmRestEvent evt)
        {
            var unit = evt.Unit;
            if (unit is null)
                return;

            if (TrainingManager.Instance.IsMemberTrained(unit.memberType))
                return;

            Bus<RestEvent>.Raise(new RestEvent(unit));
            TrainingManager.Instance.MarkMemberTrained(unit.memberType);
        }
    }
}