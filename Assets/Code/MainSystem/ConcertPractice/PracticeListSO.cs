using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using UnityEngine;

namespace Code.MainSystem.ConcertPractice
{
    [CreateAssetMenu(fileName = "PracticeList", menuName = "SO/PracticeList", order = 0)]
    public class PracticeListSO : ScriptableObject
    {
        public List<UnitDataSO> members = new List<UnitDataSO>();
    }
}