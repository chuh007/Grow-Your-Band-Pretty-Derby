using System;
using UnityEngine;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager.SubClass;
using Code.MainSystem.StatSystem.Module;
using Code.MainSystem.StatSystem.Stats;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager;
using Code.MainSystem.TraitSystem.TraitEffect;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("upgradeModule")]
        [Header("StatModule")]
        [SerializeField] private StatUpgradeModule upgradeModuleModule;
        [SerializeField] private EnsembleModule ensembleModule;

        [Header("Settings")]
        [SerializeField] private float restRecoveryAmount = 10f;

        public bool IsInitialized => _isInitialized;
        
        private bool _isInitialized;
        
        private Dictionary<MemberType, MemberStat> _memberMap;
        
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
                
                await upgradeModuleModule.Initialize();
                await ensembleModule.Initialize();
                _isInitialized = true; 
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
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
            Bus<TeamPracticeEvent>.OnEvent += HandleTeamPracticeRequested;
        }

        private void UnregisterEvents()
        {
            Bus<PracticenEvent>.OnEvent -= HandlePracticeRequested;
            Bus<ConfirmRestEvent>.OnEvent -= HandleRestRequested;
            Bus<StatIncreaseEvent>.OnEvent -= HandleSingleStatIncreaseRequested;
            Bus<TeamPracticeEvent>.OnEvent -= HandleTeamPracticeRequested;
        }

        #endregion

        #region Event Handlers

        private void HandlePracticeRequested(PracticenEvent evt)
        {
            ITraitHolder holder = TraitManager.Instance.GetHolder(evt.memberType);
            bool isSuccess = PredictMemberPractice(evt.SuccessRate, holder);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(isSuccess));

            if (!isSuccess)
            {
                foreach (var inspiration in
                         holder.GetModifiers<IInspirationSystem>()
                             .OfType<FailureBreedsSuccessEffect>())
                    inspiration.OnFailure();
                return;
            }

            float rewardValue = evt.Value;

            rewardValue = holder.GetFinalStat<ITrainingStat>(rewardValue);

            if (evt.Type == PracticenType.Personal)
                rewardValue = holder.GetFinalStat<IPracticeStat>(rewardValue);

            _operator.IncreaseMemberStat(evt.memberType, evt.statType, rewardValue);
        }

        private void HandleTeamPracticeRequested(TeamPracticeEvent evt)
        {
            if (evt.MemberConditions == null || evt.MemberConditions.Count == 0)
                return;
            bool isSuccess = ensembleModule.CheckSuccess(evt.MemberConditions);
            Bus<TeamPracticeResultEvent>.Raise(new TeamPracticeResultEvent(isSuccess));
        }

        private void HandleSingleStatIncreaseRequested(StatIncreaseEvent evt)
        {
            _operator.IncreaseMemberStat(evt.MemberType, evt.StatType, evt.Value);
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
            return _registry.GetTeamStatValue();
        }

        public bool PredictMemberPractice(float successRate, ITraitHolder holder)
        {
            upgradeModuleModule.SetCondition(successRate);
            return upgradeModuleModule.CanUpgrade(holder);
        }

        public ConditionHandler GetConditionHandler()
        {
            return _conditionHandler;
        }
        
        public EnsembleModule GetEnsembleModuleHandler()
        {
            return ensembleModule;
        }
        
        #endregion
    }
}