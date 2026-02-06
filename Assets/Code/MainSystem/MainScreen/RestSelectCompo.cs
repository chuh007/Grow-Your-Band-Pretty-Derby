using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Resting;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class RestSelectCompo : MonoBehaviour
    {
        [Header("UI")] 
        [SerializeField] private List<Button> memberButtons;
        [SerializeField] private RestResultController restResultController;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private GameObject panel;

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
            
            UpdateButtonsState();
        }

        public void Rest()
        {
            panel.SetActive(true);
            _isOpen = true;
            
            UpdateButtonsState();
        }

        public void Close()
        {
            panel.SetActive(false);
            _isOpen = false;
            _selectedUnit = null;
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
    
            float beforeHealth = _selectedUnit.currentCondition;
            float healAmount = _selectedUnit.maxCondition * 0.3f;
            float afterHealth = Mathf.Min(beforeHealth + healAmount, _selectedUnit.maxCondition);

            _selectedUnit.currentCondition = afterHealth;

            restResultController.gameObject.SetActive(true);

            await restResultController.PlayRestSequence(
                _selectedUnit,
                beforeHealth,
                afterHealth
            );
            
            TrainingManager.Instance.MarkMemberTrained(_selectedUnit.memberType);
            
            Bus<ConfirmRestEvent>.Raise(new ConfirmRestEvent(_selectedUnit));
            
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());

            healthBar.SetHealth(_selectedUnit.currentCondition, _selectedUnit.maxCondition);

            Close();
        }
        
        /// <summary>
        /// 멤버의 행동 완료 여부에 따라 버튼의 활성화 상태와 색상을 업데이트합니다.
        /// </summary>
        private void UpdateButtonsState()
        {
            if (memberButtons == null) return;

            foreach (var btn in memberButtons)
            {
                if (!Enum.TryParse(btn.name, out MemberType type))
                    continue;

                bool isTrained = TrainingManager.Instance.IsMemberTrained(type);
                
                btn.interactable = !isTrained;
                
                if (btn.image != null)
                {
                    btn.image.color = isTrained ? Color.gray : Color.white;
                }
            }
        }
        
        private void OnConfirmRest(ConfirmRestEvent evt)
        {
            var unit = evt.Unit;
            if (unit is null) 
                return;
            
            Bus<RestEvent>.Raise(new RestEvent(unit));
        }
    }
}