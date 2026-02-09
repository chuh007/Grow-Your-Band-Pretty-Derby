using UnityEngine;
using System.Linq;
using Code.Core.Bus;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager.SubClass;
using Code.MainSystem.Turn;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class TraitManager : MonoBehaviour, ITurnEndComponent
    {
        public static TraitManager Instance { get; private set; }
        
        public MemberType CurrentMember { get; private set; }

        private ITraitDatabase _database;
        private ITraitValidator _validator;
        private IPointCalculator _pointCalculator;
        private TraitInteraction _interactionManager;

        private readonly Dictionary<MemberType, ITraitHolder> _holders = new();
        

        private async void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeDependencies();
            if (_database is TraitDatabase traitDb)
                await traitDb.InitializeAsync();
            RegisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region Initialization

        private void InitializeDependencies()
        {
            _database = GetComponentInChildren<ITraitDatabase>();
            _validator = GetComponentInChildren<ITraitValidator>();
            _pointCalculator = GetComponentInChildren<IPointCalculator>();
            _interactionManager = GetComponentInChildren<TraitInteraction>();
        }

        /// <summary>
        /// CharacterTrait를 등록
        /// </summary>
        public void RegisterHolder(ITraitHolder holder)
        {
            _holders[holder.MemberType] = holder;
        }

        #endregion

        #region Event Management

        private void RegisterEvents()
        {
            Bus<TraitAddRequested>.OnEvent += HandleTraitAddRequested;
            Bus<TraitRemoveRequested>.OnEvent += HandleTraitRemoveRequested;
            Bus<TraitShowRequested>.OnEvent += HandleTraitShowRequested;
        }

        private void UnregisterEvents()
        {
            Bus<TraitAddRequested>.OnEvent -= HandleTraitAddRequested;
            Bus<TraitRemoveRequested>.OnEvent -= HandleTraitRemoveRequested;
            Bus<TraitShowRequested>.OnEvent -= HandleTraitShowRequested;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 특성 추가 요청 처리
        /// </summary>
        private void HandleTraitAddRequested(TraitAddRequested evt)
        {
            if (!TryGetHolder(evt.MemberType, out var holder))
                return;

            var traitData = _database.Get(evt.TraitType);
            if (traitData is null)
                return;
            
            var validation = _validator.CanAdd(holder, traitData);
            if (!validation.IsValid)
            {
                //Bus<TraitAddFailed>.Raise(new TraitAddFailed(validation.Message));
                return;
            }

            TryAddTrait(holder, traitData);
        }

        /// <summary>
        /// 특성 제거 요청 처리
        /// </summary>
        private void HandleTraitRemoveRequested(TraitRemoveRequested evt)
        {
            if (!TryGetHolder(evt.MemberType, out var holder))
                return;

            var target = holder.ActiveTraits
                .FirstOrDefault(t => t.Data.TraitType == evt.TraitType);

            if (target == null)
                return;

            TryRemoveTrait(holder, target);
        }

        /// <summary>
        /// 특성 보유 현황 확인 요청 처리
        /// </summary>
        private void HandleTraitShowRequested(TraitShowRequested evt)
        {
            if (!TryGetHolder(evt.MemberType, out var holder))
                return;

            CurrentMember = evt.MemberType;
            ShowTraitList(holder);
        }

        #endregion

        #region Core Logic

        /// <summary>
        /// 신규 특성 습득 시도
        /// </summary>
        private void TryAddTrait(ITraitHolder holder, TraitDataSO newTrait)
        {
            var existingTrait = holder.ActiveTraits
                .FirstOrDefault(t => t.Data.TraitType == newTrait.TraitType);
            
            if (existingTrait != null)
            {
                if (existingTrait.Data.MaxLevel == -1 ||
                    existingTrait.CurrentLevel >= existingTrait.Data.MaxLevel)
                    return;

                var prevLevel = existingTrait.CurrentLevel;
                existingTrait.LevelUp();

                Bus<TraitUpgraded>.Raise(new TraitUpgraded(CurrentMember, existingTrait, prevLevel));

                ShowTraitList(holder);
                return;
            }

            if (holder.IsAdjusting)
                return;

            holder.AddTrait(newTrait);

            var newTotal = _pointCalculator.CalculateTotalPoints(holder.ActiveTraits);
        
            if (newTotal > holder.MaxPoints)
            {
                holder.BeginAdjustment(newTrait);
                Bus<TraitOverflow>.Raise(new TraitOverflow(newTotal, holder.MaxPoints));
            }
            else
            {
                _interactionManager.ProcessAllInteractions(holder);
                ShowTraitList(holder);
            }
        }
        
        /// <summary>
        /// 특성 제거 시도
        /// </summary>
        private void TryRemoveTrait(ITraitHolder holder, ActiveTrait targetTrait)
        {
            if (!holder.IsAdjusting && !targetTrait.Data.IsRemovable)
                return;

            holder.RemoveActiveTrait(targetTrait);
            
            switch (holder.IsAdjusting)
            {
                case true when holder.TotalPoint <= holder.MaxPoints:
                    holder.EndAdjustment();
                    Bus<TraitAdjusted>.Raise(new TraitAdjusted());
                    break;
                case false:
                    ShowTraitList(holder);
                    break;
            }
        }

        /// <summary>
        /// 특성 보유 현황 표시
        /// </summary>
        private void ShowTraitList(ITraitHolder holder)
        {
            Bus<TraitShowResponded>.Raise(new TraitShowResponded(holder));
        }

        #endregion

        #region Helper Methods

        private bool TryGetHolder(MemberType memberType, out ITraitHolder holder)
        {
            return _holders.TryGetValue(memberType, out holder);
        }

        #endregion

        #region Public API

        /// <summary>
        /// 특정 멤버의 특성 정보 가져오기
        /// </summary>
        public ITraitHolder GetHolder(MemberType memberType)
        {
            return _holders.GetValueOrDefault(memberType);
        }

        /// <summary>
        /// 특정 멤버가 특정 특성을 보유하고 있는지 확인
        /// </summary>
        public bool HasTrait(MemberType memberType, TraitType traitType)
        {
            return _holders.TryGetValue(memberType, out var holder) && holder.ActiveTraits.Any(t => t.Data.TraitType == traitType);
        }

        public IReadOnlyList<TraitGroupStatus> GetTeamGroupStatus()
        {
            var groupManager = GetComponentInChildren<TraitGroupManager>();
            return groupManager.BuildGroupStatus(_holders);
        }

        #endregion
        
        public void NextScene()
        {
            SceneManager.LoadScene("Lch");
        }

        public void TurnEnd()
        {
            foreach (var holder in _holders.Values)
            {
               holder.GetModifiers<ITurnProcessListener>().FirstOrDefault()?.OnTurnPassed();
            }
        }
    }
}