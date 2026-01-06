using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingManager : MonoBehaviour
    {
        private Dictionary<MemberType, int> _trainedMembers = new();
        private bool _teamTrained = false;
        private int _curTurnTrainingCount = 0;
        public static TrainingManager Instance { get; private set; }

        private readonly MemberType[] _allMemberTypes =
        {
            MemberType.Bass,
            MemberType.Drums,
            MemberType.Guitar,
            MemberType.Piano,
            MemberType.Vocal
        };

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            foreach (var type in _allMemberTypes)
            {
                _trainedMembers.Add(type, 1);
            }
        }
        
        public int GetCurrentTrainingCount() => _curTurnTrainingCount;
        
        public void MarkMembersTrainedForTeam(IEnumerable<MemberType> members)
        {
            foreach (var member in members)
            {
                MarkMemberTrained(member);
            }
        }
        
        public bool IsMemberTrained(MemberType member)
        {
            return _trainedMembers[member] == 0;
        }

        public void MarkMemberTrained(MemberType member)
        {
            if (_trainedMembers[member] == 0)
                return;
            
            _trainedMembers[member]--;
            _curTurnTrainingCount++;
            
            Bus<MemberTrainingStateChangedEvent>.Raise(
                new MemberTrainingStateChangedEvent(member)
            );

            CheckAllMembersTrained();
        }

        public bool IsTeamTrained()
        {
            return _teamTrained;
        }

        public void MarkTeamTrained()
        {
            _teamTrained = true;
        }

        public void ResetTraining()
        {
            _curTurnTrainingCount = 0;
            foreach (var type in _allMemberTypes)
            {
                _trainedMembers[type] = 1;
            }
            _teamTrained = false;
            
            foreach (var member in _allMemberTypes)
            {
                Bus<MemberTrainingStateChangedEvent>.Raise(
                    new MemberTrainingStateChangedEvent(member)
                );
            }
        }

        
        private void CheckAllMembersTrained()
        {
            foreach (var member in _allMemberTypes)
            {
                if (_trainedMembers[member] != 0)
                    return;
            }
            
            Debug.Log("모든 멤버 훈련 완료! 턴을 넘깁니다.");
            HandleNextTrun();
        }

        private void HandleNextTrun()
        {
            ResetTraining();
            Bus<TurnEndEvent>.Raise(new TurnEndEvent());
        }
    }
}
