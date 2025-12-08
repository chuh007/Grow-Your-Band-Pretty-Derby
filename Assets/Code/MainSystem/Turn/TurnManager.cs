using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Turn
{
    
    public class TurnManager : MonoBehaviour
    {
        public event Action TurnChanged;
        public event Action TargetTurnChanged;
        public event Action GameEndEvent;
        
        [SerializeField] private int maxTurn;
        
        private int _currentTurn;
        private int _targetTurn;
        
        public int MaxTurn
        {
            get => maxTurn;
            set => maxTurn = value;
        }
        
        public int CurrentTurn
        {
            get => _currentTurn;
            set
            {
                _currentTurn = value;
                TurnChanged?.Invoke();
                if(_currentTurn >= maxTurn)
                    GameEndEvent?.Invoke();
            }
        }
        public int NextTargetTurn
        {
            get => _targetTurn;
            private set
            {
                _targetTurn = value;
                TargetTurnChanged?.Invoke();
            }
        }

        private void Awake()
        {
            Bus<TurnUseEvent>.OnEvent += HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent += HandleTurnReturn;
            Bus<TargetTurnSetEvent>.OnEvent += HandleTurnSet;
        }

        private void Start()
        {
            CurrentTurn = maxTurn;
        }

        private void OnDestroy()
        {
            Bus<TurnUseEvent>.OnEvent -= HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent -= HandleTurnReturn;
            Bus<TargetTurnSetEvent>.OnEvent -= HandleTurnSet;
        }
        
        private void HandleTurnUse(TurnUseEvent evt)
        {
            CurrentTurn += evt.Value;
        }

        private void HandleTurnReturn(TurnReturnEvent evt)
        {
            CurrentTurn -= evt.Value;
        }
        
        private void HandleTurnSet(TargetTurnSetEvent evt)
        {
            NextTargetTurn = evt.Value;
        }
    }
}