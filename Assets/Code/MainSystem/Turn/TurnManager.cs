using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using UnityEngine;

namespace Code.MainSystem.Turn
{
    public class TurnManager : MonoBehaviour, ITurnEndComponent
    {
        [Header("Data")]
        [SerializeField] private GoalFlowSO flowSO;
        
        public event Action<int> TurnChanged;
        public event Action<Goal> GoalChanged;
        
        private int _remainingTurn;
        private int _currentGoalIndex = -1;

        public int RemainingTurn 
        {
            get => _remainingTurn;
            private set
            {
                _remainingTurn = value;
                Debug.Log(value);
                TurnChanged?.Invoke(_remainingTurn);
                if (_remainingTurn <= 0)
                {
                    OnGoalFinished();
                }
            }
        }

        private void Awake()
        {
            Bus<TurnReturnEvent>.OnEvent += HandleTurnReturn;
            NextGoal();
        }
        
        private void OnDestroy()
        {
            Bus<TurnReturnEvent>.OnEvent -= HandleTurnReturn;
        }
        
        private void NextGoal()
        {
            _currentGoalIndex++;

            if (flowSO != null && _currentGoalIndex < flowSO.goals.Count)
            {
                Goal nextGoal = flowSO.goals[_currentGoalIndex];
                
                RemainingTurn = nextGoal.turn; 
                
                GoalChanged?.Invoke(nextGoal);
                TurnChanged?.Invoke(RemainingTurn);
            }
        }

        /// <summary>
        /// 현재 목표의 턴이 0이 되었을 때 호출
        /// </summary>
        private void OnGoalFinished()
        {
            // TODO 각각에 맞는 코드 실행
            Goal goal = flowSO.goals[_currentGoalIndex];
            switch (goal.type)
            {
                case GoalType.Stat:
                    // 특별한 연출 없이 합 스텟이 요구량 이상인지 검사
                    break;
                case GoalType.Busking:
                    // 버스킹 준비하는 씬으로 전환
                    Bus<ConcertStartRequested>.Raise(new ConcertStartRequested("TestSong", null, 0));
                    break;
                case GoalType.Performance:
                    // TODO 공연으로
                    break;
                case GoalType.Pen:
                    // 사용할지 모름
                    break;
            }
            NextGoal();
        }

        private void HandleTurnReturn(TurnReturnEvent evt)
        {
            RemainingTurn += evt.Value;
        }
        
        public void TurnEnd()
        {
            if (RemainingTurn > 0)
                RemainingTurn--;
        }
    }
}