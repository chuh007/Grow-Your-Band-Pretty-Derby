using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "TeamPracticeData", menuName = "Training/TeamPractice")]
    public class TeamPracticeDataSO : ScriptableObject
    {
        public string IdleImageAddressableKey;
        public string SuccessImageKey;
        public string FailImageKey;
        public string ProgressImageKey;

        public StatType practiceStatType;
        public float statIncrease = 1f;
    }


}