using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "Unit", menuName = "SO/Unit/Data", order = 0)]
    public class UnitStatSO : ScriptableObject
    {
        public string unitName;
        public Sprite unitImage;
        public List<StatData> stats;
        public int currentHealth;
        public int maxHealth;
    }
}