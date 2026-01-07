using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace Code.MainSystem.Turn.UI
{
    public class TurnCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnText;
        
        [Inject] TurnManager _turnManager;
        
        private void Awake()
        {
            _turnManager.TurnChanged += HandleTurnChange;
        }

        private void Start()
        {
            Bus<TargetTurnSetEvent>.Raise(new TargetTurnSetEvent(15));
        }

        private void OnDestroy()
        {
            _turnManager.TurnChanged -= HandleTurnChange;
        }

        private void HandleTurnChange(int value)
        {
            turnText.SetText(value.ToString());
        }
    }
}