using System;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;
using UnityEngine.Serialization;

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
        public string titleText; // 목표 설명
        public bool isTargetSet; // 목표하는 값이 있는가? 
        public Sprite icon; // 목표 아이콘 이미지
        public StatType targetType; // 목표 타입
        public int target; // 목표 요구량
        public GoalType type; // 목표의 타입
        public int turn; // 이후 몇 턴 지나면 이거 할지 
    }
    
    [CreateAssetMenu(fileName = "GoalFlow", menuName = "SO/Turn/GoalFlow", order = 0)]
    public class GoalFlowSO : ScriptableObject
    {
        public List<Goal> goals = new List<Goal>();
    }
}