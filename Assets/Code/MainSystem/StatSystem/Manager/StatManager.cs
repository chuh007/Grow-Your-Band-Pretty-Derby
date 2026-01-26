using System;
using UnityEngine;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager.SubClass;
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
        public static StatManager Instance { get; private set; }
        
        [Header("Stats Data")]
        [SerializeField] private List<MemberStat> memberStats;
        [SerializeField] private TeamStat teamStat;

        [Header("StatModule")]
        [SerializeField] private StatUpgrade upgradeModule;
        [SerializeField] private EnsembleModule ensembleModule;

        [Header("Settings")]
        [SerializeField] private float restRecoveryAmount = 10f;

        private Dictionary<MemberType, MemberStat> _memberMap;
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        
        private StatRegistry _registry;
        private StatOperator _operator;
        private ConditionHandler _conditionHandler;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponents();
            InitializeAsync();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region Initialization

        private void InitializeComponents()
        {
            _registry = new StatRegistry(memberStats, teamStat);
            _operator = new StatOperator(_registry);
            _conditionHandler = new ConditionHandler(_registry, restRecoveryAmount);
        }

        private async void InitializeAsync()
        {
            try
            {
                await _registry.InitializeAsync();
                
                await upgradeModule.Initialize();
                await ensembleModule.Initialize();
                _isInitialized = true; 
            }
            catch (Exception e)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"StatManager 초기화 실패: {e}");
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

        #region Event Handlers

        private void HandlePracticeRequested(PracticenEvent evt)
        {
            bool isSuccess = PredictMemberPractice(evt.SuccessRate);
            
            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(isSuccess));
            
            if (!isSuccess)
                return;
            
            // 성공 시 보상 적용
            ApplyPracticeReward(evt);
        }

        private void ApplyPracticeReward(PracticenEvent evt)
        {
            if (evt.Type == PracticenType.Team)
                _operator.IncreaseTeamStat(evt.Value);
            else
                _operator.IncreaseMemberStat(evt.memberType, evt.statType, evt.Value);
        }

        private void HandleTeamPracticeRequested(TeamPracticeEvent evt)
        {
            if (evt.MemberConditions == null || evt.MemberConditions.Count == 0)
                return;
            
            bool isSuccess = ensembleModule.CheckSuccess(evt.MemberConditions);
            
            Bus<TeamPracticeResultEvent>.Raise(new TeamPracticeResultEvent(isSuccess));
            
            if (isSuccess)
            {
                // TODO: 특성 시스템 완성 시 보상 추가
            }
        }

        private void HandleSingleStatIncreaseRequested(StatIncreaseEvent evt)
        {
            _operator.IncreaseMemberStat(evt.MemberType, evt.StatType, evt.Value);
        }

        private void HandleMemberAllStatIncreaseRequested(StatAllMemberStatIncreaseEvent evt)
        {
            _operator.IncreaseAllStatsForMember(evt.MemberType, evt.Value);
        }

        private void HandleAllMemberStatIncreaseRequested(StatAllIncreaseEvent evt)
        {
            _operator.IncreaseStatForAllMembers(evt.StatType, evt.Value);
        }

        private void HandleTeamStatIncreaseRequested(TeamStatIncreaseEvent evt)
        {
            _operator.IncreaseTeamStat(evt.AddValue);
        }

        private void HandleRestRequested(ConfirmRestEvent evt)
        {
            _conditionHandler.ProcessRest(evt);
        }

        #endregion

        #region Public API

        public BaseStat GetMemberStat(MemberType memberType, StatType statType)
        {
            return _registry.GetMemberStat(memberType, statType);
        }

        public BaseStat GetTeamStat(StatType statType)
        {
            return _registry.GetTeamStatValue(statType);
        }

        public bool PredictMemberPractice(float successRate)
        {
            upgradeModule.SetCondition(successRate);
            return upgradeModule.CanUpgrade();
        }

        #endregion
    }
}