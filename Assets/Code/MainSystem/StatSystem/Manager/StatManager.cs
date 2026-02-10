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
using Code.MainSystem.TraitSystem.Data;
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
            ITraitHolder holder = TraitManager.Instance.GetHolder(evt.MemberType);
            bool isSuccess = PredictMemberPractice(evt.SuccessRate, holder);

            Bus<StatUpgradeEvent>.Raise(new StatUpgradeEvent(isSuccess));

            if (!isSuccess)
            {
                foreach (var inspiration in
                         holder.GetModifiers<IInspirationSystem>()
                             .OfType<FailureBreedsSuccessEffect>())
                    inspiration.OnTrainingFailed();
                return;
            }

            float rewardValue = evt.Value;

            rewardValue = holder.GetCalculatedStat(TraitTarget.Training, rewardValue);

            if (evt.Type == PracticenType.Personal)
                rewardValue = holder.GetCalculatedStat(TraitTarget.Practice,rewardValue);

            _operator.IncreaseMemberStat(evt.MemberType, evt.StatType, rewardValue);
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
        
        /// <summary>
        /// 현재 멤버들의 컨디션 리스트를 받아 예상 합주 성공 확률을 반환합니다.
        /// </summary>
        /// <param name="memberConditions">참여하는 멤버들의 컨디션 값 리스트</param>
        /// <returns>0 ~ 100 사이의 성공 확률</returns>
        public float GetEnsembleSuccessRate(List<float> memberConditions)
        {
            return ensembleModule.CalculateSuccessRate(memberConditions);
        }

        /// <summary>
        /// 개인 훈련 예상 성공 확률을 가져옵니다.
        /// </summary>
        /// <param name="memberType">대상 멤버</param>
        /// <param name="condition">현재 컨디션 수치</param>
        public float GetPersonalSuccessRate(MemberType memberType, float condition)
        {
            ITraitHolder holder = TraitManager.Instance.GetHolder(memberType);
            return upgradeModuleModule.GetFinalSuccessRate(condition, holder);
        }

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