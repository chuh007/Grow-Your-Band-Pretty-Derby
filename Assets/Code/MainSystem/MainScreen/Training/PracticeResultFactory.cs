using System.Collections.Generic;
using UnityEngine;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen.Training
{
    public static class PracticeResultFactory
    {
        public static List<StatChangeInfo> BuildStatChanges(
            StatManager statManager,
            List<UnitDataSO> allUnits,
            float conditionCurrent,
            float conditionDelta,
            StatData teamStat,
            float teamStatDelta,
            Dictionary<(MemberType, StatType), int> statDeltaDict,
            Sprite iconNormal,
            Sprite iconDelta
        )
        {
            var list = new List<StatChangeInfo>();

            list.Add(new StatChangeInfo("컨디션", Mathf.RoundToInt(conditionDelta), iconDelta));
            list.Add(new StatChangeInfo(teamStat.statName, Mathf.RoundToInt(teamStatDelta), iconDelta));

            foreach (var unit in allUnits)
            {
                foreach (var stat in unit.stats)
                {
                    if (statManager.GetMemberStat(unit.memberType, stat.statType) is BaseStat baseStat)
                    {
                        statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta);
                        list.Add(new StatChangeInfo(stat.statName, delta, iconNormal));
                    }
                }
            }

            return list;
        }

        public static List<CommentData> BuildCommentData(
            List<UnitDataSO> allUnits,
            Dictionary<(MemberType, StatType), int> statDeltaDict
        )
        {
            var comments = new List<CommentData>();

            foreach (var unit in allUnits)
            {
                List<StatChangeInfo> changes = new();

                foreach (var stat in unit.stats)
                {
                    if (statDeltaDict.TryGetValue((unit.memberType, stat.statType), out int delta) && delta != 0)
                    {
                        changes.Add(new StatChangeInfo(stat.statName, delta, null));
                    }
                }

                if (changes.Count > 0)
                {
                    string title = $"{unit.memberType} 연습 결과";
                    string content = $"{unit.memberType}의 능력치가 변경되었습니다.";

                    comments.Add(new CommentData(title, content, changes));
                }
            }

            if (comments.Count == 0)
            {
                comments.Add(new CommentData(
                    "변화 없음",
                    "변화된 스탯이 없습니다.",
                    new List<StatChangeInfo>()
                ));
            }

            return comments;
        }
    }
}
