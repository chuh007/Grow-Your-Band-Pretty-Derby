using UnityEngine;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.MemberStats;
using Code.MainSystem.StatSystem.TeamStats;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Random = UnityEngine.Random;

namespace Code.MainSystem.StatSystem.Manager
{
    public enum MemberType
    {
        Guitar, Drums, Bass, Vocal, Piano, Team
    }
    
    public class StatManager : MonoBehaviour
    {
        [SerializeField] private List<MemberStat> memberStats;
        [SerializeField] private TeamStat teamStat;

        private Dictionary<MemberType, MemberStat> _memberMap;

        private async void Awake()
        {
            _memberMap = new Dictionary<MemberType, MemberStat>();
            
            await teamStat.InitializeAsync();
            
            foreach (var member in memberStats)
                _memberMap[member.MemberType] = member;

            Bus<PracticenEvent>.OnEvent += HandlePracticeRequested;
            Bus<ConfirmRestEvent>.OnEvent += HandleRestRequested;
            Bus<StatIncreaseEvent>.OnEvent += HandleSingleStatIncreaseRequested;
            Bus<StatAllIncreaseEvent>.OnEvent += HandleAllMemberStatIncreaseRequested;
            Bus<TeamStatIncreaseEvent>.OnEvent += HandleTeamStatIncreaseRequested;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent += HandleMemberAllStatIncreaseRequested;
        }
        
        private void OnDestroy()
        {
            Bus<PracticenEvent>.OnEvent -= HandlePracticeRequested;
            Bus<ConfirmRestEvent>.OnEvent -= HandleRestRequested;
            Bus<StatIncreaseEvent>.OnEvent -= HandleSingleStatIncreaseRequested;
            Bus<StatAllIncreaseEvent>.OnEvent -= HandleAllMemberStatIncreaseRequested;
            Bus<TeamStatIncreaseEvent>.OnEvent -= HandleTeamStatIncreaseRequested;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent -= HandleMemberAllStatIncreaseRequested;
        }
        
        private void HandlePracticeRequested(PracticenEvent evt)
        {
            if (evt.Type == PracticenType.Team)
            {
                UpgradeTeamStat(evt.SuccessRate,evt.Value);
            }
            else
            {
                UpgradeMemberStat(evt.memberType, evt.statType, evt.SuccessRate, evt.Value);
            }
        }

        #region GetStat

        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            return member?.GetStat(statType);
        }

        public BaseStat GetTeamStat(StatType statType)
        {
            return teamStat?.GetTeamStat(statType);
        }

        #endregion
        
        #region OperationStat

        private void UpgradeMemberStat(MemberType memberType, StatType statType, float successRate, float value)
        {
            var member = _memberMap.GetValueOrDefault(memberType);
            if (member is null)
                return;

            bool success = PredictMemberPractice(successRate);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(success));

            if (!success) 
                return;
            
            member.ApplyStatIncrease(statType, value);
        }
        
        private void HandleMemberAllStatIncreaseRequested(StatAllMemberStatIncreaseEvent evt)
        {
            var member = _memberMap.GetValueOrDefault(evt.MemberType);
            member?.ApplyAllStatIncrease(evt.Value);
        }
        
        public bool PredictMemberPractice(float successRate)
        {
            return Random.Range(0f, 100f) < successRate;
        }
        
        private void HandleSingleStatIncreaseRequested(StatIncreaseEvent evt)
        {
            var member = _memberMap.GetValueOrDefault(evt.MemberType);
            member?.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleAllMemberStatIncreaseRequested(StatAllIncreaseEvent evt)
        {
            foreach (var pair in memberStats)
                pair.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleTeamStatIncreaseRequested(TeamStatIncreaseEvent evt)
        {
            teamStat.ApplyTeamStatIncrease(evt.AddValue);
        }

        private void UpgradeTeamStat(float successRate, float value)
        {
            bool success = PredictMemberPractice(successRate);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(success));

            if (!success)
                return;

            teamStat.ApplyTeamStatIncrease(value);
        }
        
        private void HandleRestRequested(ConfirmRestEvent evt)
        {
            var unit = evt.Unit;
            
            if(unit is null)
                return;

            var member = _memberMap.GetValueOrDefault(unit.memberType);
            if (member is null)
                return;

            unit.currentCondition += 10;
            unit.currentCondition = Mathf.Clamp(unit.currentCondition, 0, unit.maxCondition);
        }

        #endregion
    }
}