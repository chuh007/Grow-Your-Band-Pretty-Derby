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
        
        private void Start()
        {
            TurnManager.Instance.TurnChanged += HandleTurnChange;
            HandleTurnChange(TurnManager.Instance.RemainingTurn);
        }

        private void OnDestroy()
        {
            TurnManager.Instance.TurnChanged -= HandleTurnChange;
        }

        private void HandleTurnChange(int value)
        {
            turnText.SetText(value.ToString());
        }
    }
}