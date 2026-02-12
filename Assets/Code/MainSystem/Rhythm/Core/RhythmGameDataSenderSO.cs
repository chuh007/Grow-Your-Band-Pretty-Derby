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
        [field: SerializeField] public bool IsSuccess { get; private set; }
        [field: SerializeField] public bool IsFailed { get; private set; }

        [Header("Game Result Data")]
        public bool isResultDataAvailable = false;
        public int allStatUpValue;
        public int harmonyStatUpValue;
        
        /// <summary>
        /// 모든 리듬 게임 관련 데이터를 기본값으로 초기화
        /// 인자가 없는 경우 모든 세션 데이터 비우기
        /// </summary>
        public void Initialize()
        {
            this.songId = string.Empty;
            this.concertType = ConcertType.Busking;
            this.combinedChart = null;
            this.members = null;
            this.isResultDataAvailable = false;
            this.FinalScore = 0;
            this.IsSuccess = false;
            this.IsFailed = false;
            this.allStatUpValue = 0;
            this.harmonyStatUpValue = 0;
        }

        /// <summary>
        /// 특정 곡과 멤버 정보를 사용하여 리듬 게임 데이터를 초기화
        /// </summary>
        /// <param name="songId">재생할 곡의 식별자</param>
        /// <param name="concertType">공연의 타입 (버스킹, 라이브 등)</param>
        /// <param name="members">참여하는 멤버 그룹 리스트</param>
        public void Initialize(string songId, ConcertType concertType, List<MemberGroup> members)
        {
            this.songId = songId;
            this.concertType = concertType;
            this.members = members;
            this.isResultDataAvailable = false;
            this.FinalScore = 0;
            this.IsSuccess = false;
            this.IsFailed = false;
        }
        
        /// <summary>
        /// 리듬 게임 플레이 결과를 저장
        /// </summary>
        /// <param name="finalScore">최종 획득 점수</param>
        /// <param name="isSuccess">성공 여부 (기준 점수 이상)</param>
        /// <param name="isFailed">실패 여부</param>
        public void SetResult(int finalScore, bool isSuccess, bool isFailed)
        {
            FinalScore = finalScore;
            IsSuccess = isSuccess;
            IsFailed = isFailed;
        }

        /// <summary>
        /// 저장된 결과 데이터를 소모(읽기 및 초기화)
        /// </summary>
        /// <returns>데이터 가용 여부, 전체 스탯 상승치, 하모니 스탯 상승치 튜플</returns>
        public (bool isAvailable, int allStat, int harmonyStat) ConsumeResult()
        {
            if (!isResultDataAvailable) return (false, 0, 0);

            var result = (true, allStatUpValue, harmonyStatUpValue);

            // 보상 관련 데이터 파기
            isResultDataAvailable = false;
            allStatUpValue = 0;
            harmonyStatUpValue = 0;

            return result;
        }
    }
}