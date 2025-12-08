using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Turn
{
    public class TurnCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnText;
        
        [Inject] TurnManager _turnManager;
        
        private void Awake()
        {
            _turnManager.TurnChanged += HandleTurnChange;
        }

        private void OnDestroy()
        {
            _turnManager.TurnChanged -= HandleTurnChange;
        }

        private void HandleTurnChange()
        {
            int toNextTurn = _turnManager.NextTargetTurn - _turnManager.CurrentTurn;
            turnText.SetText(toNextTurn.ToString());
        }
    }
}