using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    /// <summary>
    /// 턴별 훈련 상태를 관리하는 매니저
    /// - 멤버별 훈련 완료 여부 추적
    /// - 턴 종료 조건 체크
    /// - 훈련 카운트 관리
    /// </summary>
    public class TrainingManager : MonoBehaviour
    {
        public static TrainingManager Instance { get; private set; }

        /// <summary>
        /// 멤버별 훈련 가능 횟수 (1 = 가능, 0 = 완료)
        /// </summary>
        private Dictionary<MemberType, int> _trainedMembers = new();
        
        /// <summary>
        /// 현재 턴의 총 훈련 횟수
        /// </summary>
        private int _currentTurnTrainingCount = 0;

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeSingleton();
            SubscribeToEvents();
            InitializeAllMembers();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region Initialization

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void SubscribeToEvents()
        {
            Bus<CheckTurnEnd>.OnEvent += HandleCheckTurnEnd;
        }

        private void UnsubscribeFromEvents()
        {
            Bus<CheckTurnEnd>.OnEvent -= HandleCheckTurnEnd;
        }

        /// <summary>
        /// 모든 MemberType enum 값으로 딕셔너리 초기화
        /// 하드코딩 제거 - enum에 새 멤버 추가 시 자동 반영
        /// </summary>
        private void InitializeAllMembers()
        {
            _trainedMembers.Clear();
            
            foreach (MemberType memberType in Enum.GetValues(typeof(MemberType)))
            {
                _trainedMembers[memberType] = 1; // 1 = 훈련 가능
            }
            
            Debug.Log($"[TrainingManager] Initialized {_trainedMembers.Count} members");
        }

        #endregion

        #region Public API

        /// <summary>
        /// 현재 턴의 총 훈련 횟수 조회
        /// </summary>
        public int GetCurrentTrainingCount() => _currentTurnTrainingCount;

        /// <summary>
        /// 특정 멤버가 이번 턴에 훈련을 완료했는지 확인
        /// </summary>
        public bool IsMemberTrained(MemberType member)
        {
            if (!_trainedMembers.ContainsKey(member))
            {
                Debug.LogWarning($"[TrainingManager] Unknown member type: {member}");
                return false;
            }
            
            return _trainedMembers[member] == 0;
        }

        /// <summary>
        /// 멤버를 훈련 완료 상태로 마킹
        /// 중복 호출 방지 포함
        /// </summary>
        public void MarkMemberTrained(MemberType member)
        {
            if (!_trainedMembers.ContainsKey(member))
            {
                Debug.LogWarning($"[TrainingManager] Cannot mark unknown member: {member}");
                return;
            }
            
            // 이미 훈련 완료된 경우 스킵
            if (_trainedMembers[member] == 0)
            {
                Debug.LogWarning($"[TrainingManager] {member} is already marked as trained");
                return;
            }
            
            _trainedMembers[member] = 0;
            _currentTurnTrainingCount++;
            
            Debug.Log($"[TrainingManager] {member} marked as trained (Total: {_currentTurnTrainingCount})");
            
            RaiseMemberTrainingStateChanged(member);
        }

        /// <summary>
        /// 모든 멤버의 훈련 상태를 초기화
        /// 새로운 턴 시작 시 호출
        /// </summary>
        public void ResetTraining()
        {
            _currentTurnTrainingCount = 0;
            
            foreach (var memberType in _trainedMembers.Keys.ToList())
            {
                _trainedMembers[memberType] = 1;
            }
            
            Debug.Log("[TrainingManager] Training state reset for new turn");
            
            RaiseAllMembersStateChanged();
        }

        /// <summary>
        /// 모든 멤버가 훈련을 완료했는지 확인
        /// </summary>
        public bool CheckAllMembersTrained()
        {
            return _trainedMembers.Values.All(count => count == 0);
        }

        /// <summary>
        /// 훈련 가능한 멤버 목록 조회
        /// </summary>
        public List<MemberType> GetAvailableMembers()
        {
            return _trainedMembers
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// 훈련 완료한 멤버 목록 조회
        /// </summary>
        public List<MemberType> GetTrainedMembers()
        {
            return _trainedMembers
                .Where(kvp => kvp.Value == 0)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        #endregion

        #region Event Handling

        private void HandleCheckTurnEnd(CheckTurnEnd evt)
        {
            if (CheckAllMembersTrained())
            {
                Debug.Log("[TrainingManager] All members trained - Ending turn");
                
                Bus<TurnEndEvent>.Raise(new TurnEndEvent());
                ResetTraining();
            }
            else
            {
                var remaining = GetAvailableMembers();
                Debug.Log($"[TrainingManager] Turn not complete - {remaining.Count} members remaining");
            }
        }

        private void RaiseMemberTrainingStateChanged(MemberType member)
        {
            Bus<MemberTrainingStateChangedEvent>.Raise(
                new MemberTrainingStateChangedEvent(member)
            );
        }

        private void RaiseAllMembersStateChanged()
        {
            foreach (var memberType in _trainedMembers.Keys)
            {
                RaiseMemberTrainingStateChanged(memberType);
            }
        }

        #endregion

        #region Debug

        /// <summary>
        /// 디버그용: 현재 훈련 상태 출력
        /// </summary>
        [ContextMenu("Print Training Status")]
        public void PrintTrainingStatus()
        {
            Debug.Log("=== Training Status ===");
            Debug.Log($"Turn Training Count: {_currentTurnTrainingCount}");
            
            foreach (var kvp in _trainedMembers)
            {
                string status = kvp.Value == 0 ? "TRAINED" : "AVAILABLE";
                Debug.Log($"{kvp.Key}: {status}");
            }
        }

        #endregion
    }
}