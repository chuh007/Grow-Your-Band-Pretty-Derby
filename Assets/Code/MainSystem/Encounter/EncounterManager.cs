using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.DialogueEvents;
using UnityEngine;

namespace Code.MainSystem.Encounter
{
    /// <summary>
    /// Encounter Scene에서 Encounter 관리
    /// </summary>
    public class EncounterManager : MonoBehaviour
    {
        [SerializeField] private EncounterSenderSO encounterSender;

        private void Awake()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent += HandleDialogueStatUpgrade;
            Bus<DialogueGetSkillEvent>.OnEvent += HandleDialogueSkillGet;
            Bus<DialogueEndEvent>.OnEvent += HandleDialogueEnd;
        }
        
        private void OnDestroy()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent -= HandleDialogueStatUpgrade;
            Bus<DialogueGetSkillEvent>.OnEvent -= HandleDialogueSkillGet;
            Bus<DialogueEndEvent>.OnEvent += HandleDialogueEnd;
        }
        
        private void Start()
        {
            Bus<DialogueStartEvent>.Raise(
                new DialogueStartEvent(encounterSender.encounterData.dialogue, "")); // 여기 string은 뭐하는거람
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            
        }

        private void HandleDialogueSkillGet(DialogueGetSkillEvent evt)
        {
        }

        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
        }
    }
}