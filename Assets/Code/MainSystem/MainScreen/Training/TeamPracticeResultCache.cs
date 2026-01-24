using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public static class TeamPracticeResultCache
    {
        public static bool IsSuccess;
        public static List<UnitDataSO> SelectedMembers;
        public static Dictionary<(MemberType, StatType), int> StatDeltaDict = new();
    
        public static StatData TeamStat;
        public static float TeamStatDelta;
        public static float TeamConditionBefore;
        public static float TeamConditionDelta;
    }

}