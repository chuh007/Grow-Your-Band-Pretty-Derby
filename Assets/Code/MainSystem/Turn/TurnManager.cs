using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
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
            Bus<TurnUseEvent>.OnEvent += HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent += HandleTurnReturn;
        }

        private void Start()
        {
            RemainingTurn = maxTurn;
        }

        private void OnDestroy()
        {
            Bus<TurnUseEvent>.OnEvent -= HandleTurnUse;
            Bus<TurnReturnEvent>.OnEvent -= HandleTurnReturn;
        }

        private void HandleTurnUse(TurnUseEvent evt)
        {
            RemainingTurn -= evt.Value;
        }

        private void HandleTurnReturn(TurnReturnEvent evt)
        {
            RemainingTurn += evt.Value;
        }
    }
}