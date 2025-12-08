using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Unit", menuName = "SO/Unit/Data", order = 0)]
    public class UnitDataSO : ScriptableObject
    {
        public string unitName;
        public MemberType memberType;
        public Sprite unitImage;
        public List<StatData> stats;
        public int currentHealth;
        public int maxHealth;
        public string Lesson1TeXT;
        public string Lesson2TeXT;
    }
}