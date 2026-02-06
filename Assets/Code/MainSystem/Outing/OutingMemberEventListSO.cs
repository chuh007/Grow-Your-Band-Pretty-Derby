using System;
using System.Collections.Generic;
using Code.MainSystem.Dialogue;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.Outing
{
    [Serializable]
    public struct OutingEvent
    {
        public MemberType type;
        public OutingPlace place;
        public int weight;
        public bool isImportance;
        public string description; // 관련 설명
        public DialogueInformationSO dialogue;
    }
    
    /// <summary>
    /// 맴버의 수만큼 SO를 만들고, 그 맴버가 기본적으로 가능한 이벤트를 장소 상관없이 다 넣어둔다.
    /// </summary>
    [CreateAssetMenu(fileName = "MemberEventList", menuName = "SO/Outing/EventList", order = 0)]
    public class OutingMemberEventListSO : ScriptableObject
    {
        public MemberType type;
        public List<OutingEvent> outingEvents;
    }
}