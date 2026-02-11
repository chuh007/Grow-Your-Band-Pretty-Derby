using UnityEngine;
using System.Linq;
using Code.Core.Bus;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.Turn;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class TraitManager : MonoBehaviour, ITurnEndComponent
    {
        public static TraitManager Instance { get; private set; }
        
        public MemberType CurrentMember { get; private set; }
        
        private ITraitValidator _validator;
        private IPointCalculator _pointCalculator;
        private ITraitRegistry _registry;

        private readonly Dictionary<MemberType, ITraitHolder> _holders = new();
        private readonly Dictionary<MemberType, List<ActiveTrait>> _traitDataStorage = new();
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeDependencies();
            RegisterEvents();
        }
        
        private async void Start()
        {
            if (_registry != null)
                await _registry.Initialize();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region Initialization

        private void InitializeDependencies()
        {
            _validator = GetComponentInChildren<ITraitValidator>();
            _pointCalculator = GetComponentInChildren<IPointCalculator>();
            _registry = GetComponentInChildren<ITraitRegistry>();
        }

        /// <summary>
        /// CharacterTrait를 등록
        /// </summary>
        public void RegisterHolder(ITraitHolder holder)
        {
            _holders[holder.MemberType] = holder;
            
            if (_traitDataStorage.TryGetValue(holder.MemberType, out var savedTraits))
                holder.RestoreTraits(savedTraits);
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

            if (evt.TraitData is null) 
                return;
            
            TraitDataSO traitData = evt.TraitData;

            var validation = _validator.CanAdd(holder, traitData);
            if (!validation.IsValid) 
                return;

            TryAddTrait(holder, traitData);
        }

        /// <summary>
        /// 특성 제거 요청 처리
        /// </summary>
        private void HandleTraitRemoveRequested(TraitRemoveRequested evt)
        {
            if (!TryGetHolder(evt.MemberType, out var holder))
                return;

            int targetHash = evt.TraitData.IDHash;

            ActiveTrait target = holder.ActiveTraits
                .FirstOrDefault(t => t.Data.IDHash == targetHash);

            if (target == null) return;

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
            ActiveTrait existingTrait = holder.ActiveTraits
                .FirstOrDefault(t => t.Data.IDHash == newTrait.IDHash);
            
            if (existingTrait != null || holder.IsAdjusting)
                return;

            holder.AddTrait(newTrait);

            int newTotal = _pointCalculator.CalculateTotalPoints(holder.ActiveTraits);
        
            if (newTotal > holder.MaxPoints)
            {
                holder.BeginAdjustment(newTrait);
                Bus<TraitOverflow>.Raise(new TraitOverflow(newTotal, holder.MaxPoints));
            }
            else
            {
                ShowTraitList(holder);
            }
            
            _traitDataStorage[holder.MemberType] = holder.ActiveTraits.ToList();
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
        public bool HasTrait(MemberType memberType, int traitHash)
        {
            return _holders.TryGetValue(memberType, out var holder) && holder.ActiveTraits.Any(t => t.Data.IDHash == traitHash);
        }
        
        /// <summary>
        /// 특정 특성이 특정 태그를 가지고 있는지 확인
        /// </summary>
        public bool HasTraitTag(MemberType memberType, TraitTag traitTag)
        {
            if (!_holders.TryGetValue(memberType, out var holder)) 
                return false;
            return holder.ActiveTraits.Any(t => t.Data.TraitTag == traitTag);
        }
        
        // TODO 연결 작업시 삭제 필요
        public bool HasTrait(MemberType memberType, TraitType traitID)
        {
            return false;
        }

        public IReadOnlyList<TraitGroupStatus> GetTeamGroupStatus()
        {
            var groupManager = GetComponentInChildren<TraitGroupManager>();
            return groupManager.BuildGroupStatus(_holders);
        }

        #endregion

        public void TurnEnd()
        {
            foreach (var holder in _holders.Values)
            {
               holder.GetModifiers<ITurnProcessListener>().FirstOrDefault()?.OnTurnPassed();
            }
        }
    }
}