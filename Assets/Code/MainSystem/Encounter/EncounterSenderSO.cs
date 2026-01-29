using System;
using System.Collections.Generic;
using Code.MainSystem.Outing;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using UnityEngine;

namespace Code.MainSystem.Encounter
{
    [Serializable]
    public struct TraitData
    {
        public MemberType memberType;
        public TraitType traitType;
    }
    
    /// <summary>
    /// 인카운터 데이터 주고받는 SO
    /// </summary>
    [CreateAssetMenu(fileName = "EncounterSender", menuName = "SO/Encounter/DataSender", order = 0)]
    public class EncounterSenderSO : ScriptableObject
    {
        [Header("Encounter Input Data")]
        public EncounterDataSO encounterData; // 선택된 인카운터
        
        [Header("Encounter Result Data")]
        public List<StatVariation> changeStats; // 스텟 변경
        public List<TraitData> addedTraits; // 추가 특성
    }
}