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
            _turnManager.TargetTurnChanged += HandleTurnChange;
        }

        private void Start()
        {
            Bus<TargetTurnSetEvent>.Raise(new TargetTurnSetEvent(15));
        }

        private void OnDestroy()
        {
            _turnManager.TurnChanged -= HandleTurnChange;
            _turnManager.TargetTurnChanged -= HandleTurnChange;
        }

        private void HandleTurnChange()
        {
            int toNextTurn = _turnManager.MaxTurn - _turnManager.CurrentTurn + _turnManager.NextTargetTurn;
            turnText.SetText(toNextTurn.ToString());
        }
    }
}