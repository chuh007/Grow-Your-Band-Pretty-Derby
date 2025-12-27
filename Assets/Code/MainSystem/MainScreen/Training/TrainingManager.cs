using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingManager : MonoBehaviour
    {
        private HashSet<MemberType> _trainedMembers = new();
        private bool _teamTrained = false;

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

        public bool IsMemberTrained(MemberType member)
        {
            return _trainedMembers.Contains(member);
        }

        public void MarkMemberTrained(MemberType member)
        {
            if (!_trainedMembers.Contains(member))
            {
                _trainedMembers.Add(member);
                CheckAllMembersTrained();
            }
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
            _trainedMembers.Clear();
            _teamTrained = false;
        }
        
        private void CheckAllMembersTrained()
        {
            foreach (var member in _allMemberTypes)
            {
                if (!_trainedMembers.Contains(member))
                    return;
            }
            
            Debug.Log("모든 멤버 훈련 완료! 턴을 넘깁니다.");
            HandleNextTrun();
        }

        private void HandleNextTrun()
        {
            ResetTraining();
            Bus<TurnUseEvent>.Raise(new TurnUseEvent(1));
        }
    }
}
