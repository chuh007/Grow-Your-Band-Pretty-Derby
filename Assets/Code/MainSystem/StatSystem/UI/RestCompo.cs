using TMPro;
using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.StatSystem.UI
{
    public class RestCompo : MonoBehaviour
    {
        [SerializeField] private StatManager statManager;
        [SerializeField] private TextMeshProUGUI conditionText;

        private UnitDataSO _currentUnit;

        public void Init(UnitDataSO unit)
        {
            _currentUnit = unit;
        }

        public void Rest()
        {
            if (_currentUnit == null)
            {
                return;
            }

                Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
            Bus<RestEvent>.Raise(new RestEvent(_currentUnit.memberType));
            UpdateConditionText();
        }

        private void UpdateConditionText()
        {
            if (conditionText is null || _currentUnit is null)
                return;

            var stat = statManager.GetMemberStat(_currentUnit.memberType,StatType.Condition);
            conditionText.SetText(
                $"{stat.CurrentValue}/{stat.MaxValue}"
            );
            
            Debug.Log($"{_currentUnit.name}: {conditionText.text}");
        }
    }
}