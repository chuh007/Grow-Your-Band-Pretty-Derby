using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.DialogueEvents;
using Code.MainSystem.Outing;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.Cutscene.DialogCutscene
{
    /// <summary>
    /// DialogCutscene에서 다이얼로그의 시작과 끝, 결과창 띄우기 등을 담당
    /// </summary>
    public class DialogCutsceneController : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO resultSender;
        [SerializeField] private Transform uiRoot;
        
        private void Awake()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent += HandleDialogueStatUpgrade;
            Bus<DialogueGetSkillEvent>.OnEvent += HandleDialogueSkillGet;
            Bus<DialogueEndEvent>.OnEvent += HandleDialogueEnd;
            resultSender.changeStats.Clear();
            resultSender.addedTraits.Clear();
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
                new DialogueStartEvent(resultSender.selectedEvent, "")); // 여기 string은 뭐하는거람
        }
        
        public async UniTask PlayOutingSequence()
        {
            var resultPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Outing/UI/Result");
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<OutingResultUI>();
            resultUI.ShowResultUI("DialogCutscene");
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            resultSender.changeStats.Add
                (new StatVariation{targetStat = evt.StatType, variation = evt.StatValue});
        }
        
        private void HandleDialogueSkillGet(DialogueGetSkillEvent evt)
        {
            resultSender.addedTraits.Add(evt.TraitType);
        }
        
        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
            _ = PlayOutingSequence();
        }
    }
}