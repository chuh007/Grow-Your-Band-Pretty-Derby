using UnityEngine;

namespace Code.MainSystem.Turn
{
    public class TurnManager : MonoBehaviour
    {
        
        
        private int _remainingTurn;

        public int RemainingTurn
        {
            get => _remainingTurn;
            set
            {
                _remainingTurn = value;
                
            }
        }
    }
}