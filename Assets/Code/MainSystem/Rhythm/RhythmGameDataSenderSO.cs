using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Rhythm
{
    [CreateAssetMenu(fileName = "RhythmGameDataSender", menuName = "SO/Rhythm/DataSender", order = 0)]
    public class RhythmGameDataSenderSO : ScriptableObject
    {
        [Header("Game Input Data")]
        // 선택된 멤버를 담아서 넘겨주기. 버튼 누른 시점에 그 리스트 배껴오면 될듯
        public List<UnitDataSO> members = new List<UnitDataSO>();
        
        [Header("Game Result Data")]
        public int allStatUpValue;
        public int harmonyStatUpValue;
        
    }
}