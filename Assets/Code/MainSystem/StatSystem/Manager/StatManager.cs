using System;
using UnityEngine;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using System.Linq;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Module;
using Code.MainSystem.StatSystem.Stats;

namespace Code.MainSystem.StatSystem.Manager
{
    public enum MemberType
    {
        Guitar, Drums, Bass, Vocal, Piano, Team
    }
    
    public class StatManager : MonoBehaviour
    {
        [Header("Stats Data")]
        [SerializeField] private List<MemberStat> memberStats;
        [SerializeField] private TeamStat teamStat;

        [Header("StatModule")]
        [SerializeField] private StatUpgrade upgradeModule;
        [SerializeField] private EnsembleModule ensembleModule;

        [Header("Settings")]
        [SerializeField] private float restRecoveryAmount = 10f;

        private Dictionary<MemberType, MemberStat> _memberMap;
        
        private void Awake()
        {
            InitializeMemberMap();
            RegisterEvents();
            InitializeModule();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region Init

        private async void InitializeMemberMap()
        {
            try
            {
                await teamStat.InitializeAsync();

                _memberMap = new Dictionary<MemberType, MemberStat>();
                foreach (var member in memberStats.Where(member => member != null))
                    _memberMap.TryAdd(member.MemberType, member);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"InitializeMemberMap 실패: {e}");
#endif
            }
        }
        
        private async void InitializeModule()
        {
            try
            {
                await upgradeModule.Initialize();
                await ensembleModule.Initialize();
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"InitializeModule 실패: {e}");
#endif
            }
        }

        #endregion

        #region Event Management

        private void RegisterEvents()
        {
            Bus<PracticenEvent>.OnEvent += HandlePracticeRequested;
            Bus<ConfirmRestEvent>.OnEvent += HandleRestRequested;
            Bus<StatIncreaseEvent>.OnEvent += HandleSingleStatIncreaseRequested;
            Bus<StatAllIncreaseEvent>.OnEvent += HandleAllMemberStatIncreaseRequested;
            Bus<TeamStatIncreaseEvent>.OnEvent += HandleTeamStatIncreaseRequested;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent += HandleMemberAllStatIncreaseRequested;
            Bus<TeamPracticeEvent>.OnEvent += HandleTeamPracticeRequested;
        }

        private void UnregisterEvents()
        {
            Bus<PracticenEvent>.OnEvent -= HandlePracticeRequested;
            Bus<ConfirmRestEvent>.OnEvent -= HandleRestRequested;
            Bus<StatIncreaseEvent>.OnEvent -= HandleSingleStatIncreaseRequested;
            Bus<StatAllIncreaseEvent>.OnEvent -= HandleAllMemberStatIncreaseRequested;
            Bus<TeamStatIncreaseEvent>.OnEvent -= HandleTeamStatIncreaseRequested;
            Bus<StatAllMemberStatIncreaseEvent>.OnEvent -= HandleMemberAllStatIncreaseRequested;
            Bus<TeamPracticeEvent>.OnEvent -= HandleTeamPracticeRequested;
        }

        #endregion
        
        #region Event Handlers & Core Logic
        
        private void HandleTeamPracticeRequested(TeamPracticeEvent evt)
        {
            if (evt.MemberConditions == null || evt.MemberConditions.Count == 0)
                return;

            bool isSuccess = ensembleModule.CheckSuccess(evt.MemberConditions);
            Bus<TeamPracticeResultEvent>.Raise(new TeamPracticeResultEvent(isSuccess));
            
            if (!isSuccess)
                return;
            
            //특성 시스템 완성시 스탯 올려주는 기능 필요
        }
        
        public bool PredictMemberPractice(float successRate)
        {
            upgradeModule.SetCondition(successRate);
           return upgradeModule.CanUpgrade();
        }

        private void HandlePracticeRequested(PracticenEvent evt)
        {
            bool isSuccess = PredictMemberPractice(evt.SuccessRate);
            
            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(isSuccess));
            
            if (!isSuccess)
                return;
            
            ApplyPracticeSuccess(evt);
        }

        private void ApplyPracticeSuccess(PracticenEvent evt)
        {
            if (evt.Type == PracticenType.Team)
                teamStat?.ApplyTeamStatIncrease(evt.Value);
            else
                if (_memberMap.TryGetValue(evt.memberType, out var member))
                    member.ApplyStatIncrease(evt.statType, evt.Value);
        }

        private void HandleSingleStatIncreaseRequested(StatIncreaseEvent evt)
        {
            if (_memberMap.TryGetValue(evt.MemberType, out var member))
                member.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleMemberAllStatIncreaseRequested(StatAllMemberStatIncreaseEvent evt)
        {
            if (_memberMap.TryGetValue(evt.MemberType, out var member))
                member.ApplyAllStatIncrease(evt.Value);
        }

        private void HandleAllMemberStatIncreaseRequested(StatAllIncreaseEvent evt)
        {
            foreach (var member in memberStats)
                member?.ApplyStatIncrease(evt.StatType, evt.Value);
        }
        
        private void HandleTeamStatIncreaseRequested(TeamStatIncreaseEvent evt)
        {
            teamStat?.ApplyTeamStatIncrease(evt.AddValue);
        }
        
        private void HandleRestRequested(ConfirmRestEvent evt)
        {
            var unit = evt.Unit;
            if (unit is null)
                return;
            
            if (!_memberMap.ContainsKey(unit.memberType))
                return;

            unit.currentCondition = Mathf.Clamp(unit.currentCondition + restRecoveryAmount, 0, unit.maxCondition);
        }

        #endregion

        #region GetStat
        
        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
        {
            return _memberMap.GetValueOrDefault(memberType)?.GetStat(statType);
        }

        public BaseStat GetTeamStat(StatType statType)
        {
            return teamStat?.GetTeamStat(statType);
        }

        #endregion
    }
}