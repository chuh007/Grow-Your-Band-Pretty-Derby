using System;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Outing
{
    /// <summary>
    /// 외출 씬 작업 담당하는 메니저(싱글턴 아님)
    /// </summary>
    public class OutingManager : MonoBehaviour
    {
        [SerializeField] private OutingResultSenderSO resultSender;
        [SerializeField] private Transform uiRoot;
        
        private void Awake()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent += HandleDialogueStatUpgrade;
            Bus<DialogueEndEvent>.OnEvent += HandleDialogueEnd;
            resultSender.changeStats.Clear();
        }

        private void OnDestroy()
        {
            Bus<DialogueStatUpgradeEvent>.OnEvent -= HandleDialogueStatUpgrade;
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
            Debug.Log(resultPrefab);
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<OutingResultUI>();
            resultUI.ShowResultUI();
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            resultSender.changeStats.Add
                (new StatVariation{targetStat = evt.StatType, variation = evt.StatValue});
        }
        
        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
            Debug.Log("ㄴ");
            _ = PlayOutingSequence();
        }
    }
}