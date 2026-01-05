using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Turn
{
    
    public class TurnManager : MonoBehaviour
    {
        public event Action<int> TurnChanged;
        public event Action TurnZero;
        
        [SerializeField] private int maxTurn;
        [SerializeField] private int _remainingTurn;

        public int RemainingTurn
        {
            get => _remainingTurn;
            set
            {
                _remainingTurn = value;
                if (value <= 0)
                {
                    TurnZero?.Invoke();
                }
                TurnChanged?.Invoke(value);
            }
        }

        private void Awake()
        {
            Bus<TurnEndEvent>.OnEvent += HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent += HandleTurnReturn;
        }

        private void Start()
        {
            RemainingTurn = maxTurn;
        }

        private void OnDestroy()
        {
            Bus<TurnEndEvent>.OnEvent -= HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent -= HandleTurnReturn;
        }

        private void HandleTurnUse(TurnEndEvent evt)
        {
            RemainingTurn--;
        }

        private void HandleTurnReturn(TurnReturnEvent evt)
        {
            RemainingTurn += evt.Value;
        }
    }
}