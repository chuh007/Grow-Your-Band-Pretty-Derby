using System;
using System.Threading.Tasks;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Turn
{
    public class TurnManager : MonoBehaviour, ITurnEndComponent
    {
        [Header("Data")]
        [SerializeField] private GoalFlowSO flowSO;
        [SerializeField] private RhythmGameDataSenderSO dataSO;
        
        public static TurnManager Instance { get; private set; }
        
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
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private async void Start()
        {
            while (StatManager.Instance != null && !StatManager.Instance.IsInitialized)
            {
                await Task.Delay(100);
            }
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
                
                if (nextGoal.icon != null)
                {
                    BaseStat stat = GetStat(nextGoal.targetType);
                    Bus<TargetSettingEvent>.Raise(new TargetSettingEvent
                    (nextGoal.titleText, nextGoal.icon, 
                        (nextGoal.target - stat.CurrentValue).ToString(), nextGoal.isTargetSet));
                }
                
                GoalChanged?.Invoke(nextGoal);
                RemainingTurn = nextGoal.turn;
            }
        }

        /// <summary>
        /// 현재 목표의 턴이 0이 되었을 때 호출
        /// </summary>
        private void OnGoalFinished()
        {
            Goal goal = flowSO.goals[_currentGoalIndex];
            switch (goal.type)
            {
                case GoalType.Stat:
                    // 특별한 연출 없이 특정 스텟이 요구량 이상인지 검사
                    BaseStat stat = GetStat(goal.targetType);
                    if (stat.CurrentValue < goal.target)
                    {
                        Debug.Log("아이고들어가면큰일나죠이거는");
                        // TODO 끝나는 인카운터 호출
                    }
                    else
                    {
                        // TODO 이벤트가 있을지도
                    }
                    break;
                case GoalType.Busking:
                    // 버스킹 준비하는 씬으로 전환
                    Debug.Log("BusKing");
                    //Bus<ConcertStartRequested>.Raise(new ConcertStartRequested("TestSong", dataSO.members));
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
            {
                RemainingTurn--;
                if (flowSO != null && _currentGoalIndex < flowSO.goals.Count)
                {
                    Goal nextGoal = flowSO.goals[_currentGoalIndex];
                
                    if (nextGoal.icon != null)
                    {
                        BaseStat stat = GetStat(nextGoal.targetType);
                        Bus<TargetSettingEvent>.Raise(new TargetSettingEvent
                        (nextGoal.titleText, nextGoal.icon, 
                            (nextGoal.target - stat.CurrentValue).ToString(), nextGoal.isTargetSet));
                    }
                }
            }
        }

        private BaseStat GetStat(StatType type)
            => StatManager.Instance.GetTeamStat(type);
    }
}