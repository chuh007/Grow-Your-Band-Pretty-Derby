using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.MainSystem.Turn
{
    public class TurnTester : MonoBehaviour
    {
        [SerializeField] private int targetTurn;
        
        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                Bus<TurnUseEvent>.Raise(new TurnUseEvent(1));
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Bus<TurnReturnEvent>.Raise(new TurnReturnEvent(1));
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Bus<TargetTurnSetEvent>.Raise(new TargetTurnSetEvent(targetTurn));
            }
        }
    }
}