using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class PersonalTrainingType : ITrainingType, IStatChangeProvider
    {
        private PersonalpracticeDataSO data;

        public PersonalTrainingType(PersonalpracticeDataSO practiceData)
        {
            data = practiceData;
        }

        public string GetIdleImageKey() => data.IdleImageAddressableKey;

        public string GetResultImageKey(bool isSuccess)
            => isSuccess ? data.SuccseImageAddressableKey : data.FaillImageAddressableKey;

        public string GetProgressImageKey() => data.ProgressImageAddresableKey;

        public string GetIdlePrefabKey()
        {
            return "Training/SD/Idle"; 
        }

        public string GetResultUIPrefabKey()
        {
            return "Training/UI/Result"; 
        }

        public List<(string name, Sprite icon, int baseValue, int delta)> GetStatChanges(
            UnitDataSO unit,
            StatManager statManager,
            bool isSuccess)
        {
            StatType targetType = data.PracticeStatType;
            var statList = new List<(string, Sprite, int, int)>();

            for (int i = 0; i < unit.stats.Count && statList.Count < 4; i++)
            {
                var stat = unit.stats[i];
                var memberStat = statManager.GetMemberStat(unit.memberType, stat.statType);

                int delta = (isSuccess && stat.statType == targetType)
                    ? Mathf.RoundToInt(data.statIncrease)
                    : 0;

                statList.Add((stat.statName, stat.statIcon, Mathf.RoundToInt(memberStat.CurrentValue), delta));
            }

            return statList;
        }
    }
}