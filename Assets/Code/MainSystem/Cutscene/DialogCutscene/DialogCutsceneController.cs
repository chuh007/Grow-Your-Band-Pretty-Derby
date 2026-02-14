using System.Collections.Generic;
using System.Text;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.DialogueEvents;
using Code.MainSystem.Outing;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Code.MainSystem.Cutscene.DialogCutscene
{
    /// <summary>
    /// DialogCutscene에서 다이얼로그의 시작과 끝, 결과창 띄우기 등을 담당
    /// </summary>
    public class DialogCutsceneController : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO resultSender;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Transform uiRoot;
        
        // 포메팅용. 스텟 이름은 일반 텍스트. 증가량은 녹색
        private static readonly string RESULT_FORMAT = "{0} <color=#00FFAC>+{1}</color>";
        private static readonly string SKILL_RESULT_FORMAT = "<color=#00FFAC>{0}</color> 획득";
        
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
                new DialogueStartEvent(resultSender.selectedEvent, "")); // 여기 string은 뭐하는거람
        }
        
        public async UniTask PlayOutingSequence()
        {
            var resultPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Outing/UI/Result");
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<OutingResultUI>();
            resultUI.ShowResultUI("DialogCutscene");
            
            StringBuilder resultBuilder = new StringBuilder();
            
            var aggregatedStats = new Dictionary<StatType, int>();
            
            foreach (var stat in resultSender.changeStats)
            {
                if (aggregatedStats.ContainsKey(stat.targetStat))
                {
                    aggregatedStats[stat.targetStat] += stat.variation;
                }
                else
                {
                    aggregatedStats[stat.targetStat] = stat.variation;
                }
            }

            foreach (var entry in aggregatedStats)
            {
                resultBuilder.Append(string.Format(RESULT_FORMAT, entry.Key.ToString(), entry.Value));
                resultBuilder.AppendLine();
            }

            foreach (var skill in resultSender.addedTraits)
            {
                resultBuilder.Append(string.Format(SKILL_RESULT_FORMAT, skill.targetTrait.TraitName));
            }

            resultText.SetText(resultBuilder.ToString());
        }
        
        private void HandleDialogueStatUpgrade(DialogueStatUpgradeEvent evt)
        {
            resultSender.changeStats.Add
                (new StatVariation{targetStat = evt.Stat.targetStat, targetMember = evt.Stat.targetMember, variation = evt.Stat.variation});
        }
        
        private void HandleDialogueSkillGet(DialogueGetSkillEvent evt)
        {
            resultSender.addedTraits.Add(evt.TraitType);
        }
        
        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
            foreach (StatVariation var in resultSender.changeStats)
            {
                BaseStat stat;
                if (var.targetStat == StatType.TeamHarmony)
                    stat = StatManager.Instance.GetTeamStat(StatType.TeamHarmony);
                else
                    stat = StatManager.Instance.GetMemberStat(var.targetMember, var.targetStat);
                
                Bus<StatIncreaseDecreaseEvent>.Raise(
                    new StatIncreaseDecreaseEvent(var.variation > 0, 
                        var.variation.ToString(), stat.StatIcon, stat.StatName));
            }
            _ = PlayOutingSequence();
        }
    }
}