using Code.MainSystem.StatSystem.Events;
using UnityEngine;

namespace Code.MainSystem.StatSystem.Manager.SubClass
{
    public class ConditionHandler
    {
        private readonly StatRegistry _registry;
        private readonly float _restRecoveryAmount;

        public ConditionHandler(StatRegistry registry, float restRecoveryAmount = 10f)
        {
            _registry = registry;
            _restRecoveryAmount = restRecoveryAmount;
        }

        public void ProcessRest(ConfirmRestEvent evt)
        {
            var unit = evt.Unit;
            if (unit is null || !_registry.TryGetMember(unit.memberType, out _))
                return;

            unit.currentCondition = Mathf.Clamp(
                unit.currentCondition + _restRecoveryAmount, 
                0, 
                unit.maxCondition
            );
        }
    }
}