using System;
using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training 
{
    /// <summary>
    /// 개별 멤버의 스탯 변화를 저장하는 직렬화 가능한 클래스
    /// </summary>
    [Serializable]
    public class MemberStatDelta
    {
        public MemberType memberType;
        public StatType statType;
        public int delta;
    }

    /// <summary>
    /// 팀 연습 결과 데이터를 저장하는 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "TeamPracticeResult", menuName = "SO/Training/TeamPracticeResult")]
    public class TeamPracticeResultData : ScriptableObject
    {
        [Header("결과 기본 정보")]
        public bool isSuccess;
        public List<UnitDataSO> selectedMembers = new();
        
        [Header("스탯 변화")]
        public List<MemberStatDelta> statDeltas = new(); // Dictionary 대신 List 사용
        
        [Header("팀 스탯")]
        public StatData teamStat;
        public int teamStatDelta;
        
        [Header("컨디션")]
        public float teamConditionCurrent;
        public float teamConditionDelta;

        /// <summary>
        /// 결과 데이터 초기화
        /// </summary>
        public void Clear()
        {
            isSuccess = false;
            selectedMembers.Clear();
            statDeltas.Clear();
            teamStat = null;
            teamStatDelta = 0;
            teamConditionCurrent = 0f;
            teamConditionDelta = 0f;
        }

        /// <summary>
        /// 결과 데이터 설정
        /// </summary>
        public void SetResult(
            bool success,
            List<UnitDataSO> members,
            Dictionary<(MemberType, StatType), int> statDeltaDict,
            StatData tStat,
            int tStatDelta,
            float conditionCurrent,
            float conditionDelta)
        {
            isSuccess = success;
            selectedMembers = new List<UnitDataSO>(members);
            
            // Dictionary를 List로 변환
            statDeltas.Clear();
            foreach (var kvp in statDeltaDict)
            {
                statDeltas.Add(new MemberStatDelta
                {
                    memberType = kvp.Key.Item1,
                    statType = kvp.Key.Item2,
                    delta = kvp.Value
                });
            }
            
            teamStat = tStat;
            teamStatDelta = tStatDelta;
            teamConditionCurrent = conditionCurrent;
            teamConditionDelta = conditionDelta;
        }

        /// <summary>
        /// 특정 멤버의 스탯 변화량 조회
        /// </summary>
        public int GetStatDelta(MemberType member, StatType stat)
        {
            var delta = statDeltas.Find(d => d.memberType == member && d.statType == stat);
            return delta?.delta ?? 0;
        }

        /// <summary>
        /// Dictionary 형태로 반환 (기존 코드 호환성)
        /// </summary>
        public Dictionary<(MemberType, StatType), int> GetStatDeltaDict()
        {
            var dict = new Dictionary<(MemberType, StatType), int>();
            foreach (var delta in statDeltas)
            {
                dict[(delta.memberType, delta.statType)] = delta.delta;
            }
            return dict;
        }

        /// <summary>
        /// 유효한 데이터인지 검증
        /// </summary>
        public bool IsValid()
        {
            return selectedMembers != null && selectedMembers.Count > 0;
        }
    }
}