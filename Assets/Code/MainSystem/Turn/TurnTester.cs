using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.MainSystem.Turn
{
    public class TurnTester : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                Bus<TurnEndEvent>.Raise(new TurnEndEvent());
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                Bus<TurnReturnEvent>.Raise(new TurnReturnEvent(1));
            }
        }
    }
}