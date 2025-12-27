using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Outing
{
    public class OutingManager : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;

        private void Awake()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent += HandleDialogueStatUpgrade;
        }

        private void OnDestroy()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent -= HandleDialogueStatUpgrade;
        }
        
        private void Start()
        {
            Bus<DialogueStartEvent>.Raise(
                new DialogueStartEvent(resultSender.selectedEvent, "")); // 여기 string은 뭐하는거람
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            resultSender.changeStats.Add
                (new StatVariation{targetStat = evt.StatType, variation = evt.StatValue});
        }
    }
}