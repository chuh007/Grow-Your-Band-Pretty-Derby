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

        public StatUIUpdater(List<TextMeshProUGUI> names, List<TextMeshProUGUI> values, List<Image> icons, StatManager manager)
        {
            nameTexts = names;
            valueTexts = values;
            iconImages = icons;
            statManager = manager;
        }

        public void UpdateAll(UnitDataSO unit)
        {
            int count = Mathf.Min(unit.stats.Count, nameTexts.Count);

            for (int i = 0; i < count; i++)
            {
                var stat = unit.stats[i];
                nameTexts[i].SetText(stat.statName);

                var valueText = $"{GetCurrentValue(unit, stat.statType)} / {GetMaxValue(unit, stat.statType)}";
                valueTexts[i].SetText(valueText);

                if (i < iconImages.Count)
                {
                    iconImages[i].sprite = stat.statIcon;
                }
            }
        }

        public void UpdateStatValues(UnitDataSO unit)
        {
            int count = Mathf.Min(unit.stats.Count, valueTexts.Count);

            for (int i = 0; i < count; i++)
            {
                var stat = unit.stats[i];
                var valueText = $"{GetCurrentValue(unit, stat.statType)} / {GetMaxValue(unit, stat.statType)}";
                valueTexts[i].SetText(valueText);
            }
        }

        private float GetCurrentValue(UnitDataSO unit, StatType type)
        {
            return type == StatType.TeamHarmony
                ? statManager.GetTeamStat(type).CurrentValue
                : statManager.GetMemberStat(unit.memberType, type).CurrentValue;
        }

        private float GetMaxValue(UnitDataSO unit, StatType type)
        {
            return type == StatType.TeamHarmony
                ? statManager.GetTeamStat(type).MaxValue
                : statManager.GetMemberStat(unit.memberType, type).MaxValue;
        }
    }
}
