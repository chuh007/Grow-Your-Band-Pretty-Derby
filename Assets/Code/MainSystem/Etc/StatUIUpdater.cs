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
            if (unit == null || unit.stats == null)
            {
                Debug.LogError("[StatUIUpdater] UpdateAll: unit or unit.stats is null");
                return;
            }

            for (int i = 0; i < unit.stats.Count; i++)
            {
                if (i >= valueTexts.Count || valueTexts[i] == null)
                {
                    Debug.LogWarning($"[StatUIUpdater] valueTexts[{i}] is null or out of range");
                    continue;
                }

                var stat = unit.stats[i];

                BaseStat statData = stat.statType == StatType.TeamHarmony
                    ? statManager.GetTeamStat(stat.statType)
                    : statManager.GetMemberStat(unit.memberType, stat.statType);

                if (statData == null)
                {
                    Debug.LogError($"[StatUIUpdater] Stat is NULL : {stat.statType}");
                    valueTexts[i].SetText("0");
                    continue;
                }

                if (i < nameTexts.Count && nameTexts[i] != null)
                {
                    nameTexts[i].SetText(stat.statName);
                }
                
                valueTexts[i].SetText($"{statData.CurrentValue}");
                
                if (i < iconImages.Count && iconImages[i] != null)
                {
                    iconImages[i].sprite = statData.CurrentRankIcon;
                }
            }
            
            if (unit.teamStat != null)
            {
                BaseStat teamStatData = statManager.GetTeamStat(unit.teamStat.statType);
                
                if (teamStatData != null)
                {
                    if (4 < nameTexts.Count && nameTexts[4] != null)
                    {
                        nameTexts[4].SetText(teamStatData.StatName);
                    }
                    
                    if (4 < valueTexts.Count && valueTexts[4] != null)
                    {
                        valueTexts[4].SetText($"{teamStatData.CurrentValue}");
                    }
                    
                    if (4 < iconImages.Count && iconImages[4] != null)
                    {
                        iconImages[4].sprite = teamStatData.CurrentRankIcon;
                    }
                }
            }
        }

        public void PreviewStat(UnitDataSO unit, StatType targetType, float increase)
        {
            if (unit == null || unit.stats == null)
            {
                Debug.LogError("[StatUIUpdater] PreviewStat: unit or unit.stats is null");
                return;
            }

            for (int i = 0; i < unit.stats.Count; i++)
            {
                if (i >= valueTexts.Count || valueTexts[i] == null) continue;

                var stat = unit.stats[i];

                BaseStat statData = stat.statType == StatType.TeamHarmony
                    ? statManager.GetTeamStat(stat.statType)
                    : statManager.GetMemberStat(unit.memberType, stat.statType);

                if (statData == null)
                {
                    Debug.LogError($"[StatUIUpdater] Stat is NULL : {stat.statType}");
                    continue;
                }

                if (stat.statType == targetType)
                {
                    valueTexts[i].SetText($"<color=green>{statData.CurrentValue + increase} (+{increase})</color>");
                }
                else
                {
                    valueTexts[i].SetText($"{statData.CurrentValue}");
                }
            }
            
            if (unit.teamStat != null)
            {
                BaseStat teamStatData = statManager.GetTeamStat(unit.teamStat.statType);
                
                if (teamStatData != null && 4 < valueTexts.Count && valueTexts[4] != null)
                {
                    if (unit.teamStat.statType == targetType)
                    {
                        valueTexts[4].SetText($"<color=green>{teamStatData.CurrentValue + increase} (+{increase})</color>");
                    }
                    else
                    {
                        valueTexts[4].SetText($"{teamStatData.CurrentValue}");
                    }
                }
            }
        }
    }
}