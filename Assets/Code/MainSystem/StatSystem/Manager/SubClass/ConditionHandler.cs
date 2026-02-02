using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager;
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
            UnitDataSO unit = evt.Unit;
            if (unit is null || !_registry.TryGetMember(unit.memberType, out _))
                return;

            var holder = TraitManager.Instance.GetHolder(unit.memberType);

            float finalRecovery =
                holder.GetFinalStat<IConditionStat>(_restRecoveryAmount);

            unit.currentCondition = Mathf.Clamp(
                unit.currentCondition + finalRecovery,
                0,
                unit.maxCondition
            );
        }

        public float ModifyConditionCost(MemberType memberType, float baseCost)
        {
            var holder = TraitManager.Instance.GetHolder(memberType);
            return holder?.GetFinalStat<IConditionStat>(baseCost) ?? baseCost;
        }
    }
}