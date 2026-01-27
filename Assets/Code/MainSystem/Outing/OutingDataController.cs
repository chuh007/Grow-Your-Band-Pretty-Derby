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
    // 외출 종료시 데이터 받아서 처리해줌
    public class OutingDataController : MonoBehaviour, ITurnStartComponent
    {
        [SerializeField] private OutingMemberEventListSO[] outingMemberEventLists;
        [SerializeField] private OutingResultSenderSO outingResultSender;
        
        private Dictionary<(MemberType, OutingPlace), DialogueInformationSO> _memberRealOuting;

        private Dictionary<(MemberType, OutingPlace), List<OutingEvent>> _outingEvents;
        
        private void Awake()
        {
            Bus<AddOutingEvent>.OnEvent += HandleAddOuting;
            Bus<OutingEndEvent>.OnEvent += HandleOutingEnd;
            
            _memberRealOuting = new Dictionary<(MemberType, OutingPlace), DialogueInformationSO>();
            _outingEvents = new Dictionary<(MemberType, OutingPlace), List<OutingEvent>>();

            foreach (MemberType type in Enum.GetValues(typeof(MemberType)))
            {
                foreach (OutingPlace place in Enum.GetValues(typeof(OutingPlace)))
                {
                    _memberRealOuting.Add((type, place), null);
                    _outingEvents.Add((type, place), new List<OutingEvent>());
                }
            }
            
            foreach (var list in outingMemberEventLists)
            {
                foreach (var outingEvent in list.outingEvents)
                {
                    _outingEvents[(outingEvent.type, outingEvent.place)].Add(outingEvent);
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
            _outingEvents[(evt.Event.type, evt.Event.place)].Add(evt.Event);
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
            var events = _outingEvents[(memberType, place)];
            
            foreach (var outingEvent in events)
            {
                if (outingEvent.isImportance)
                {
                    _memberRealOuting[(memberType, place)] = outingEvent.dialogue;
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
                    _memberRealOuting[(memberType, place)] = outingEvent.dialogue;
                    return;
                }
                
                rand -= outingEvent.weight;
            }
        }
        
        public DialogueInformationSO GetMemberOutingData(MemberType memberType, OutingPlace place)
            => _memberRealOuting.GetValueOrDefault((memberType, place));
    }
}