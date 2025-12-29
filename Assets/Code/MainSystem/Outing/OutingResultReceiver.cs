using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;

namespace Code.MainSystem.Outing
{
    // 외출 종료시 데이터 받아서 처리해줌
    public class OutingResultReceiver : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO outingResultSender;
        private void Awake()
        {
            Bus<OutingEndEvent>.OnEvent += HandleOutingEnd;
        }

        private void OnDestroy()
        {
            Bus<OutingEndEvent>.OnEvent -= HandleOutingEnd;
        }

        private void HandleOutingEnd(OutingEndEvent evt)
        {
            Debug.Log("Outing End");
            foreach (var stat in outingResultSender.changeStats)
            {
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    (outingResultSender.targetMember.memberType, stat.targetStat, stat.variation));
            }
        }
    }
}