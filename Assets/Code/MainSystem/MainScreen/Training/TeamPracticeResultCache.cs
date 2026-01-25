using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.Training
{
    public static class TeamPracticeResultCache
    {
        public static bool IsSuccess;
        public static List<UnitDataSO> SelectedMembers = new();
        public static Dictionary<(MemberType memberType, StatType statType), int> StatDeltaDict = new();

        public static StatData TeamStat;
        public static float TeamStatDelta;

        public static float TeamConditionCurrent;
        public static float TeamConditionDelta;
    }
}