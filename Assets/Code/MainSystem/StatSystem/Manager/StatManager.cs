using UnityEngine;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
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

            Bus<PracticenEvent>.OnEvent += HandlePractice;
            Bus<RestEvent>.OnEvent += HandleRest;
            Bus<StatIncreaseEvent>.OnEvent += HandleStatUpgrade;
            Bus<StatAllIncreaseEvent>.OnEvent += HandleStatAllUpgrade;
            Bus<TeamStatIncreaseEvent>.OnEvent += HandleTeamStatUpgrade;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent += HandleAllMemberStatIncrease;
        }

        private void HandleRest(RestEvent evt)
        {
            Rest(evt.Unit);
        }

        private void HandlePractice(PracticenEvent evt)
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
        
        private void OnDestroy()
        {
            Bus<PracticenEvent>.OnEvent -= HandlePractice;
            Bus<RestEvent>.OnEvent -= HandleRest;
            Bus<StatIncreaseEvent>.OnEvent -= HandleStatUpgrade;
            Bus<StatAllIncreaseEvent>.OnEvent -= HandleStatAllUpgrade;
            Bus<TeamStatIncreaseEvent>.OnEvent -= HandleTeamStatUpgrade;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent -= HandleAllMemberStatIncrease;
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
        
        public IReadOnlyDictionary<MemberType, BaseStat> GetAllMemberStat(StatType statType)
        {
            Dictionary<MemberType, BaseStat> result = new();

            foreach (var pair in _memberMap)
            {
                BaseStat stat = pair.Value.GetStat(statType);
                if (stat != null)
                    result.Add(pair.Key, stat);
            }

            return result;
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
        
        private void HandleAllMemberStatIncrease(StatAllMemberStatIncreaseEvent evt)
        {
            var member = _memberMap.GetValueOrDefault(evt.MemberType);
            member?.ApplyAllStatIncrease(evt.Value);
        }
        
        public bool PredictMemberPractice(float successRate)
        {
            return Random.Range(0f, 100f) < successRate;
        }
        
        private void HandleStatUpgrade(StatIncreaseEvent evt)
        {
            var member = _memberMap.GetValueOrDefault(evt.MemberType);
            if (member is null)
                return;
            
            member.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleStatAllUpgrade(StatAllIncreaseEvent evt)
        {
            foreach (var pair in memberStats)
                pair.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleTeamStatUpgrade(TeamStatIncreaseEvent evt)
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
        
        private void Rest(UnitDataSO unit)
        {
            if (unit is null)
                return;

            var member = _memberMap.GetValueOrDefault(unit.memberType);
            if (member is null)
                return;

            int recoverValue = CalculateRestRecover(member);

            unit.currentCondition += recoverValue;
            unit.currentCondition = Mathf.Min(unit.currentCondition, unit.maxCondition);
        }

        private int CalculateRestRecover(AbstractStats target)
        {
            BaseStat mental = target.GetStat(StatType.Mental);
            return mental == null ? 10 : 10 + (int)(mental.CurrentValue * 0.5f);
        }

        #endregion
    }
}