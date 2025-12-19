using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

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
                valueTexts[i].SetText($"{statManager.GetMemberStat(unit.memberType, stat.statType).CurrentValue} / {statManager.GetMemberStat(unit.memberType, stat.statType).MaxValue}");
                iconImages[i].sprite = stat.statIcon;
            }
        }

        public void UpdateStatValues(UnitDataSO unit)
        {
            int count = Mathf.Min(unit.stats.Count, valueTexts.Count);

            for (int i = 0; i < count; i++)
            {
                var stat = unit.stats[i];
                valueTexts[i].SetText($"{statManager.GetMemberStat(unit.memberType, stat.statType).CurrentValue} / {statManager.GetMemberStat(unit.memberType, stat.statType).MaxValue}");
            }
        }
    }
}