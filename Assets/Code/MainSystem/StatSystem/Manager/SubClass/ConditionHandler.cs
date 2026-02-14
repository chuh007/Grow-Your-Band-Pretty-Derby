using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.TraitSystem.Data;
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

        /// <summary>
        /// 휴식 시 컨디션 회복 처리
        /// </summary>
        public void ProcessRest(ConfirmRestEvent evt)
        {
            UnitDataSO unit = evt.Unit;
            if (unit is null || !_registry.TryGetMember(unit.memberType, out _))
                return;

            ITraitHolder holder = TraitManager.Instance.GetHolder(unit.memberType);
            
            holder.ExecuteTrigger(TraitTrigger.OnRestStarted);
            
            float bonusMultiplier = holder.QueryTriggerValue(TraitTrigger.CalcStatMultiplier, TraitTarget.Condition);
            float finalMultiplier = bonusMultiplier > 0 ? (1f + bonusMultiplier) : 1f;
            
            float finalRecovery = holder.GetCalculatedStat(TraitTarget.PracticeCondition, _restRecoveryAmount) * finalMultiplier;

            unit.currentCondition = Mathf.Clamp(
                unit.currentCondition + finalRecovery,
                0,
                unit.maxCondition
            );
        }

        /// <summary>
        /// 행동 시 소모되는 컨디션 수치 보정
        /// </summary>
        public float ModifyConditionCost(MemberType memberType, float baseCost)
        {
            ITraitHolder holder = TraitManager.Instance.GetHolder(memberType);
            
            float trainingBonus = holder.QueryTriggerValue(TraitTrigger.CalcTrainingReward);
            baseCost += trainingBonus;
            
            float costMultiplier = holder.QueryTriggerValue(TraitTrigger.CalcConditionCost);
            float finalMultiplier = costMultiplier > 0 ? costMultiplier : 1f;
            
            return holder.GetCalculatedStat(TraitTarget.Condition, baseCost) * finalMultiplier;
        }
    }
}