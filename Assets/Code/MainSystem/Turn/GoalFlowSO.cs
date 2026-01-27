using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.Turn
{
    public enum GoalType
    {
        Stat, // 합 스텟 일정량?
        Pen, // 팬 수치 일정량 (팬 기능 쓴다면)
        Busking, // 버스킹
        Performance // 공연
    }

    [Serializable]
    public struct Goal
    {
        public string text; // 목표 설명
        public GoalType type; // 목표의 타입
        public int turn; // 이후 몇 턴 지나면 이거 할지 
    }
    
    [CreateAssetMenu(fileName = "GoalFlow", menuName = "SO/GoalFlow", order = 0)]
    public class GoalFlowSO : ScriptableObject
    {
        public List<Goal> goals = new List<Goal>();
    }
}