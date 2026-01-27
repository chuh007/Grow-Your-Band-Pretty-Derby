using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.Serialization;

using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.Core
{
    [System.Serializable]
    public class MemberGroup
    {
        public List<MemberType> Members = new List<MemberType>();
    }

    [CreateAssetMenu(fileName = "RhythmGameDataSender", menuName = "SO/Rhythm/DataSender", order = 0)]
    public class RhythmGameDataSenderSO : ScriptableObject
    {
        [Header("Game Input Data")]
        public string songId;
        public ConcertType concertType;
        public List<NoteData> combinedChart;
        
        public List<MemberGroup> members = new List<MemberGroup>();
        
        [Header("Game Result Data")]
        public bool isResultDataAvailable;
        public int allStatUpValue;
        public int harmonyStatUpValue;
        
    }
}