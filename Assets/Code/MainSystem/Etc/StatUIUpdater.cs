using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Etc
{
    public class StatUIUpdater
    {
        private readonly List<TextMeshProUGUI> nameTexts;
        private readonly List<TextMeshProUGUI> valueTexts;
        private readonly List<Image> iconImages;
        private readonly StatManager statManager;

        public StatUIUpdater(
            List<TextMeshProUGUI> names,
            List<TextMeshProUGUI> values,
            List<Image> icons,
            StatManager manager)
        {
            nameTexts = names;
            valueTexts = values;
            iconImages = icons;
            statManager = manager;
        }

        public void UpdateAll(UnitDataSO unit)
        {
            if (unit == null || unit.stats == null) return;

            for (int i = 0; i < unit.stats.Count; i++)
            {
                if (i >= valueTexts.Count || valueTexts[i] == null) continue;

                var stat = unit.stats[i];

                BaseStat statData = stat.statType == StatType.TeamHarmony
                    ? statManager.GetTeamStat(stat.statType)
                    : statManager.GetMemberStat(unit.memberType, stat.statType);

                if (statData == null)
                {
                    Debug.LogError($"Stat is NULL : {stat.statType}");
                    valueTexts[i].SetText("0 / 0");
                    continue;
                }

                nameTexts[i].SetText(stat.statName);
                valueTexts[i].SetText($"{statData.CurrentValue} / {statData.MaxValue}");
                if (iconImages[i] != null)
                    iconImages[i].sprite = stat.statIcon;
            }

            BaseStat teamstatData = statManager.GetTeamStat(unit.teamStat.statType);
            nameTexts[4].SetText(teamstatData.StatName);
            valueTexts[4].SetText($"{teamstatData.CurrentValue} / {teamstatData.MaxValue}");
            if (iconImages[4] != null)
                iconImages[4].sprite = teamstatData.StatIcon;
        }

        public void PreviewStat(UnitDataSO unit, StatType targetType, float increase)
        {
            if (unit == null || unit.stats == null) return;

            for (int i = 0; i < unit.stats.Count; i++)
            {
                if (i >= valueTexts.Count || valueTexts[i] == null) continue;

                var stat = unit.stats[i];

                BaseStat statData = stat.statType == StatType.TeamHarmony
                    ? statManager.GetTeamStat(stat.statType)
                    : statManager.GetMemberStat(unit.memberType, stat.statType);

                if (statData == null)
                {
                    Debug.LogError($"Stat is NULL : {stat.statType}");
                    continue;
                }

                if (stat.statType == targetType)
                {
                    valueTexts[i].SetText($"<color=green>{statData.CurrentValue + increase} (+{increase})</color> / {statData.MaxValue}");
                }
                else
                {
                    valueTexts[i].SetText($"{statData.CurrentValue} / {statData.MaxValue}");
                }
            }

            BaseStat teamstatData = statManager.GetTeamStat(unit.teamStat.statType);
            
            if (unit.teamStat.statType == targetType)
            {
                valueTexts[4].SetText($"<color=green>{teamstatData.CurrentValue + increase} (+{increase})</color> / {teamstatData.MaxValue}");
            }
            else
            {
                valueTexts[4].SetText($"{teamstatData.CurrentValue} / {teamstatData.MaxValue}");
            }
        }
    }
}
