using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{

    [System.Serializable]
    public class PersonalComment
    {
        public string comment;
        public string thoughts;
        public Sprite icon;
    }
    
    [CreateAssetMenu(fileName = "Personal", menuName = "SO/Practice/Personal", order = 0)]
    public class PersonalpracticeDataSO : ScriptableObject
    {
        public StatType PracticeStatType;
        public string PracticeStatName;
        public string PersonalpracticeDescription;
        public float StaminaReduction;
        public float statIncrease;
        
        public PersonalComment PersonalsuccessComment;
        public PersonalComment PersonalfaillComment;
    }
}