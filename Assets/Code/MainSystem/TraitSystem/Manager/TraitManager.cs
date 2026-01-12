using UnityEngine;
using Code.Core.Bus;
using Reflex.Attributes;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Runtime;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager
{
    public class TraitManager : MonoBehaviour
    {
        [Inject] private ITraitPointCalculator _calculator;
        
        private readonly Dictionary<MemberType, ITraitHolder> _holders = new();
        private ITraitHolder _pendingHolder;
        private TraitDataSO _pendingNewTrait;

        public bool Adjusting { get; private set; }

        private void Awake()
        {
            RegisterEvents();
            RegisterHolders();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }
        
        #region Event Management

        private void RegisterEvents()
        {
            Bus<TraitAddRequested>.OnEvent += OnTraitAddRequested;
            Bus<TraitRemoveRequested>.OnEvent += OnTraitRemoveRequested;
        }
        
        private void UnregisterEvents()
        {
            Bus<TraitAddRequested>.OnEvent -= OnTraitAddRequested;
            Bus<TraitRemoveRequested>.OnEvent -= OnTraitRemoveRequested;
        }

        #endregion

        /// <summary>
        /// Scene에 있는 모든 CharacterTrait를 등록
        /// </summary>
        private void RegisterHolders()
        {
            var holders = FindObjectsByType<CharacterTrait>(FindObjectsSortMode.None);
            foreach (var holder in holders)
                _holders[holder.MemberType] = holder;
        }

        /// <summary>
        /// 특성 추가 요청 처리
        /// </summary>
        private void OnTraitAddRequested(TraitAddRequested evt)
        {
            if (!_holders.TryGetValue(evt.MemberType, out var holder))
                return;

            TryAddTrait(holder, evt.NewTrait);
        }

        /// <summary>
        /// 특성 제거 요청 처리
        /// </summary>
        private void OnTraitRemoveRequested(TraitRemoveRequested evt)
        {
            if (!_holders.TryGetValue(evt.MemberType, out var holder))
                return;

            TryRemoveTrait(holder, evt.TargetTrait);
        }

        /// <summary>
        /// 신규 특성 습득 시도
        /// </summary>
        private void TryAddTrait(ITraitHolder holder, TraitDataSO newTrait)
        {
            int currentTotal = _calculator.CalculateTotalPoint(holder);
            int afterTotal = currentTotal + newTrait.Point;
            
            if (afterTotal <= holder.MaxPoints)
                holder.AddTrait(newTrait);
            else
                StartAdjustment(holder, newTrait);
        }

        /// <summary>
        /// 특성 제거 시도
        /// </summary>
        private void TryRemoveTrait(ITraitHolder holder, ActiveTrait targetTrait)
        {
            if (!Adjusting || _pendingHolder != holder)
                return;

            if (!RemoveTrait(holder, targetTrait))
                return;

            int currentTotal = _calculator.CalculateTotalPoint(holder);
            int afterTotal = currentTotal + _pendingNewTrait.Point;

            if (afterTotal > holder.MaxPoints)
                return;
            
            holder.AddTrait(_pendingNewTrait);
            CompleteAdjustment();
        }

        /// <summary>
        /// 특성 제거 로직
        /// </summary>
        private bool RemoveTrait(ITraitHolder holder, ActiveTrait targetTrait)
        {
            if (targetTrait.Data.IsRemove) 
                return false;
            
            holder.RemoveActiveTrait(targetTrait);
            return true;
        }
        
        /// <summary>
        /// 조정 모드 시작
        /// </summary>
        private void StartAdjustment(ITraitHolder holder, TraitDataSO newTrait)
        {
            Adjusting = true;
            _pendingHolder = holder;
            _pendingNewTrait = newTrait;
            
            int currentTotal = _calculator.CalculateTotalPoint(holder);
            Bus<TraitOverflow>.Raise(new TraitOverflow(currentTotal + newTrait.Point, holder.MaxPoints));
        }

        /// <summary>
        /// 조정 완료
        /// </summary>
        private void CompleteAdjustment()
        {
            Adjusting = false;
            _pendingHolder = null;
            _pendingNewTrait = null;
            Bus<TraitAdjusted>.Raise(new TraitAdjusted());
        }
    }
}