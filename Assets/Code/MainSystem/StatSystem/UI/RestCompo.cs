using TMPro;
using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.UI
{
    public class RestCompo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI conditionText;

        private UnitDataSO _currentUnit;

        public void Init(UnitDataSO unit)
        {
            _currentUnit = unit;
            UpdateConditionText();
        }

        public void Rest()
        {
            if (_currentUnit == null)
            {
                Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
                return;
            }

            Bus<RestEvent>.Raise(new RestEvent(_currentUnit));
            UpdateConditionText();
        }

        private void UpdateConditionText()
        {
            if (_currentUnit == null || conditionText == null)
                return;

            conditionText.SetText(
                $"{_currentUnit.currentCondition}/{_currentUnit.maxCondition}"
            );
        }
    }
}