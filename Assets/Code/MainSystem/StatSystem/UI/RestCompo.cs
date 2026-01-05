using UnityEngine;
using Code.Core.Bus;
using Code.MainSystem.MainScreen;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.UI
{
    public class RestCompo : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;

        private UnitDataSO _currentUnit;

        public void Init(UnitDataSO unit)
        {
            _currentUnit = unit;

            if (healthBar != null)
                healthBar.SetHealth(_currentUnit.currentCondition, _currentUnit.maxCondition);
        }

        public void Rest()
        {
            if (_currentUnit == null)
            {
                return;
            }

            //Bus<SelectRequiredEvent>.Raise(new SelectRequiredEvent());
            float beforeCondition = _currentUnit.currentCondition;
            Bus<RestEvent>.Raise(new RestEvent(_currentUnit));

            float afterCondition = _currentUnit.currentCondition;
            float recoveredAmount = afterCondition - beforeCondition;

            if (healthBar != null && recoveredAmount > 0f)
                healthBar.ApplyHealth(-recoveredAmount);
        }
    }
}