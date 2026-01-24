using System;
using System.Collections.Generic;
using Code.MainSystem.Dialogue;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.TraitSystem.Data;
using UnityEngine;

namespace Code.MainSystem.Outing
{
    [Serializable]
    public struct StatVariation
    {
        public StatType targetStat;
        public int variation;
    }
    
    [CreateAssetMenu(fileName = "OutingDataSender", menuName = "SO/Outing/DataSender", order = 0)]
    public class OutingResultSenderSO : ScriptableObject
    {
        [Header("Outing Input Data")]
        public UnitDataSO targetMember; // 들어온 대상
        public DialogueInformationSO selectedEvent; // 기획상 턴 시작시에 어떤 이벤트 발생할지 정해놓고 한다 함.

        [Header("Game Result Data")]
        public List<StatVariation> changeStats;
        public List<TraitType> addedTraits;
    }
}