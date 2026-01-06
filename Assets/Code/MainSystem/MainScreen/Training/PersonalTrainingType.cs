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

        public PersonalTrainingType(PersonalpracticeDataSO data)
        {
            this.data = data;
        }

        public string GetIdleImageKey() => data.IdleImageAddressableKey;
        public string GetResultImageKey(bool isSuccess) => isSuccess ? data.SuccseImageAddressableKey : data.FaillImageAddressableKey;
        public string GetProgressImageKey() => data.ProgressImageAddresableKey;

        public string GetIdlePrefabKey() => "Training/SD/Idle";
        public string GetResultUIPrefabKey() => "Training/UI/Result";

        public (string name, Sprite icon, int baseValue, int delta)
            GetTeamStatResult(TeamStatManager teamStatManager, bool isSuccess)
        {
            var stat = teamStatManager.GetStat(data.practiceTeamStatType);
            int delta = isSuccess ? Mathf.RoundToInt(data.teamStatIncrease) : 0;

            return (
                stat.displayName,
                stat.icon,
                Mathf.RoundToInt(stat.currentValue),
                delta
            );
        }
    }

}