using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.StatSystem.Module.Data
{
    [CreateAssetMenu(fileName = "Upgrade data", menuName = "SO/Stat/Upgrade data", order = 0)]
    public class UpgradeData : ScriptableObject
    {
        [Header("Success Value (%)")]
        public List<float> conditionSuccessRates = new List<float>
        {
            30f,  // 매우 나쁨 (0-20%)
            50f,  // 나쁨 (20-40%)
            70f,  // 보통 (40-60%)
            85f,  // 좋음 (60-80%)
            95f   // 매우 좋음 (80-100%)
        };
    }
}