using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Personal", menuName = "SO/Practice/Personal", order = 0)]
    public class PersonalpracticeDataSO : ScriptableObject
    {
        public StatType PracticeStatType;
        public string PracticeStatName;
        public string personalpracticeDescription;
        public float StaminaReduction;
        public float statIncrease;
    }
}