using System;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.DialogueEvents;
using Code.MainSystem.Cutscene.DialogCutscene;
using Code.MainSystem.Outing;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Encounter
{
    /// <summary>
    /// Encounter Scene에서 Encounter 관리
    /// </summary>
    public class EncounterController : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO dialogSender;
        [SerializeField] private Transform uiRoot;
        
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
                new DialogueStartEvent(dialogSender.selectedEvent, "")); // 여기 string은 뭐하는거람
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            dialogSender.changeStats.Add
                (new StatVariation{targetStat = evt.StatType, variation = evt.StatValue});
        }

        private void HandleDialogueSkillGet(DialogueGetSkillEvent evt)
        {
            dialogSender.addedTraits.Add(evt.TraitType);
        }

        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
            _ = PlayOutingSequence();
        }
        
        public async UniTask PlayOutingSequence()
        {
            var resultPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Outing/UI/Result");
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<OutingResultUI>();
            resultUI.ShowResultUI("EncounterScene");
        }
    }
}