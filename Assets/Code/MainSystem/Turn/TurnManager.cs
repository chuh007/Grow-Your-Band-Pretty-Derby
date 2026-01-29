using System;
using System.Threading.Tasks;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Encounter;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Turn
{
    public class TurnManager : MonoBehaviour, ITurnEndComponent, ITurnStartComponent
    {
        [Header("Data")]
        [SerializeField] private GoalFlowSO flowSO;
        
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
                TurnChanged?.Invoke(_remainingTurn);
            }
        }
        
        private void Awake()
        {
            Bus<TurnReturnEvent>.OnEvent += HandleTurnReturn;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                RemainingTurn = flowSO.goals[0].turn;
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
                UpdateTarget();
                
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
                // 특별한 연출 없이 특정 스텟이 요구량 이상인지 검사
                case GoalType.Stat:
                {
                    BaseStat stat = GetStat(goal.targetType);
                    if (stat.CurrentValue < goal.target)
                    {
                        Debug.Log("스텟실패");
                        Bus<EncounterCheckEvent>.Raise(new EncounterCheckEvent(EncounterConditionType.StatCaseFall));
                    }
                    else
                    {
                        Debug.Log("스텟성공");
                        // TODO 이벤트가 있을지도
                    }
                    break;
                }

                case GoalType.Busking:
                {
                    // 버스킹 준비하는 씬으로 전환
                    BaseStat stat = GetStat(goal.targetType);
                    if (stat.CurrentValue < goal.target)
                    {
                        Debug.Log("버스킹실패");
                        Bus<EncounterCheckEvent>.Raise(new EncounterCheckEvent(EncounterConditionType.BuskingCaseFall));
                    }
                    else
                    {
                        Debug.Log("버스킹성공");
                        Bus<ConcertStartRequested>.Raise(new ConcertStartRequested("TestSong", ConcertType.Busking,
                            RhythmGameConsts.MEMBERS_GROUP));
                    }
                    break;
                }
                    
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
            Debug.Log("TurnEnd");
            if (RemainingTurn > 0)
            {
                RemainingTurn--;
                if (flowSO != null && _currentGoalIndex < flowSO.goals.Count)
                {
                    UpdateTarget();
                }
            }
        }

        private BaseStat GetStat(StatType type)
            => StatManager.Instance.GetTeamStat(type);

        public void TurnStart()
        {
            if (RemainingTurn <= 0)
                OnGoalFinished();
        }

        public void UpdateTarget()
        {
            Goal nextGoal = flowSO.goals[_currentGoalIndex];
            
            BaseStat stat = GetStat(nextGoal.targetType);
            Bus<TargetSettingEvent>.Raise(new TargetSettingEvent
            (nextGoal.titleText, nextGoal.icon, 
                (nextGoal.target - stat.CurrentValue), nextGoal.isTargetSet));
        }
    }
}