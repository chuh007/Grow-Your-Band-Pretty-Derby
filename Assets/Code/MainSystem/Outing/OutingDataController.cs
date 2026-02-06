using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Dialogue;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.Turn;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.MainSystem.Outing
{
    // 외출 종료시 데이터 받아서 처리해주고, 턴 시작 시점에서 외출 설정
    public class OutingDataController : MonoBehaviour, ITurnStartComponent
    {
        [SerializeField] private OutingMemberEventListSO[] outingMemberEventLists;
        [SerializeField] private OutingResultSenderSO outingResultSender;
        
        public Dictionary<(MemberType, OutingPlace), OutingEvent> MemberRealOuting { get; private set; }

        public Dictionary<(MemberType, OutingPlace), List<OutingEvent>> OutingEvents { get; private set; }
        
        private void Awake()
        {
            Bus<AddOutingEvent>.OnEvent += HandleAddOuting;
            Bus<OutingEndEvent>.OnEvent += HandleOutingEnd;
            
            MemberRealOuting = new Dictionary<(MemberType, OutingPlace), OutingEvent>();
            OutingEvents = new Dictionary<(MemberType, OutingPlace), List<OutingEvent>>();

            foreach (MemberType type in Enum.GetValues(typeof(MemberType)))
            {
                foreach (OutingPlace place in Enum.GetValues(typeof(OutingPlace)))
                {
                    MemberRealOuting.Add((type, place), default);
                    OutingEvents.Add((type, place), new List<OutingEvent>());
                }
            }
            
            foreach (var list in outingMemberEventLists)
            {
                foreach (var outingEvent in list.outingEvents)
                {
                    OutingEvents[(outingEvent.type, outingEvent.place)].Add(outingEvent);
                }
            }
        }

        private void Start()
        {
            foreach (MemberType type in Enum.GetValues(typeof(MemberType)))
            {
                foreach (OutingPlace place in Enum.GetValues(typeof(OutingPlace)))
                {
                    SetMemberOutingData(type, place);
                }
            }
        }

        private void OnDestroy()
        {
            Bus<OutingEndEvent>.OnEvent -= HandleOutingEnd;
            Bus<AddOutingEvent>.OnEvent -= HandleAddOuting;
        }
        
        private void HandleAddOuting(AddOutingEvent evt)
        {
            OutingEvents[(evt.Event.type, evt.Event.place)].Add(evt.Event);
            // SetMemberOutingData(evt.Event.type, evt.Event.place);
        }
        
        private void HandleOutingEnd(OutingEndEvent evt)
        {
            Debug.Log("Outing End");
            foreach (var stat in outingResultSender.changeStats)
            {
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    (outingResultSender.targetMember.memberType, stat.targetStat, stat.variation));
            }
            foreach (var trait in outingResultSender.addedTraits)
            {
                Bus<TraitAddRequested>.Raise(new TraitAddRequested
                    (outingResultSender.targetMember.memberType, trait));
            }
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
        }

        public void TurnStart()
        {
            foreach (MemberType type in Enum.GetValues(typeof(MemberType)))
            {
                foreach (OutingPlace place in Enum.GetValues(typeof(OutingPlace)))
                {
                    SetMemberOutingData(type, place);
                }
            }
        }

        private void SetMemberOutingData(MemberType memberType, OutingPlace place)
        {
            int sum = 0;
            var events = OutingEvents[(memberType, place)];
            foreach (var outingEvent in events)
            {
                if (outingEvent.isImportance)
                {
                    MemberRealOuting[(memberType, place)] = outingEvent;
                    return;
                }
                sum += outingEvent.weight;
            }
            if (sum <= 0) return;
            
            int rand = Random.Range(0, sum);
            
            foreach (var outingEvent in events)
            {
                if (rand < outingEvent.weight)
                {
                    MemberRealOuting[(memberType, place)] = outingEvent;
                    return;
                }
                
                rand -= outingEvent.weight;
            }
        }
        
        public OutingEvent GetMemberOutingData(MemberType memberType, OutingPlace place)
            => MemberRealOuting.GetValueOrDefault((memberType, place));
    }
}