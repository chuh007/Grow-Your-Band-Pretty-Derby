using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Unit", menuName = "SO/Unit/Data")]
    public class UnitDataSO : ScriptableObject
    {
        public string unitName;
        public MemberType memberType;
        public string spriteAddressableKey;
        public List<StatData> stats;
        public List<PersonalpracticeDataSO> personalPractices;
        public float maxCondition;
        public float currentCondition;
        public StatData TeamStat;
        public string TeamIdleSpriteKey;
        public string TeamSuccseSpriteKey;
        public string TeamFaillSpriteKey;
        public string TeamProggresSpriteKey;
    }

}