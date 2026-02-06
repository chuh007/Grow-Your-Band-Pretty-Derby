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
        
        public List<MemberGroup> members;
        
        [Header("Result Data")]
        [field: SerializeField] public int FinalScore { get; private set; }
        [field: SerializeField] public bool IsSuccess { get;  set; }
        [field: SerializeField] public bool IsFailed { get;  set; }

        [Header("Game Result Data")]
        public bool isResultDataAvailable = false;
        public int allStatUpValue;
        public int harmonyStatUpValue;
        
        public void Initialize(string songId, ConcertType concertType, List<MemberGroup> members)
        {
            this.songId = songId;
            this.concertType = concertType;
            this.members = members;
            this.isResultDataAvailable = false;
            this.FinalScore = 0;
            this.IsSuccess = false;
        }
        
        public void SetResult(int finalScore, bool isSuccess, bool isFailed)
        {
            FinalScore = finalScore;
            IsSuccess = isSuccess;
            IsFailed = isFailed;
        }

        public (bool isAvailable, int allStat, int harmonyStat) ConsumeResult()
        {
            if (!isResultDataAvailable) return (false, 0, 0);

            var result = (true, allStatUpValue, harmonyStatUpValue);

            // 데이터 파기 (초기화)
            isResultDataAvailable = false;
            allStatUpValue = 0;
            harmonyStatUpValue = 0;

            return result;
        }
    }
}