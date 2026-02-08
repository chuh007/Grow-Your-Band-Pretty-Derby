using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.Turn;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingManager : MonoBehaviour, ITurnStartComponent
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
                Bus<CheckTurnEnd>.OnEvent += HandleCheckTurnEnd;
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

        public bool IsMemberTrained(MemberType member)
        {
            return _trainedMembers[member] == 0;
        }

        public void MarkMemberTrained(MemberType member)
        {
            if (_trainedMembers[member] == 0)
                return;
            
            _trainedMembers[member] = 0;
            _curTurnTrainingCount++;
            
            Bus<MemberTrainingStateChangedEvent>.Raise(
                new MemberTrainingStateChangedEvent(member)
            );
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

        private bool CheckAllMembersTrained()
        {
            foreach (var member in _allMemberTypes)
            {
                if (_trainedMembers[member] > 0)
                    return false;
            }
            
            return true;
        }
        
        public void TurnStart()
        {
            ResetTraining();
        }
        
        private void HandleCheckTurnEnd(CheckTurnEnd evt)
        {
            if (CheckAllMembersTrained())
            {
                Bus<TurnEndEvent>.Raise(new TurnEndEvent());
                ResetTraining();
            }
        }
    }
}