using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.CutsceneEvents;
using Code.Core.Bus.GameEvents.OutingEvents;
using Code.Core.Bus.GameEvents.TraitEvents;
using Code.MainSystem.StatSystem.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Cutscene.DialogCutscene
{
    /// <summary>
    /// 다이얼로그 컷신으로 보내는 것을 관리하는 메니저
    /// 갔다 왔을때 데이터 적용도 담당함
    /// </summary>
    public class DialogCutsceneDataManager : MonoBehaviour
    {
        [SerializeField] private DialogCutsceneSenderSO dialogSender;
        
        private void Awake()
        {
            Bus<DialogCutscenePlayEvent>.OnEvent += HandleDialogCutscenePlay;
        }

        private void Start()
        {
            foreach (var stat in dialogSender.changeStats)
            {
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    (stat.targetMember, stat.targetStat, stat.variation));
            }
            foreach (var trait in dialogSender.addedTraits)
            {
                Bus<TraitAddRequested>.Raise(new TraitAddRequested
                    (trait.targetMember, trait.targetStat));
            }
            
            dialogSender.changeStats.Clear();
            dialogSender.addedTraits.Clear();
        }

        private void OnDestroy()
        {
            Bus<DialogCutscenePlayEvent>.OnEvent -= HandleDialogCutscenePlay;
        }
        
        private void HandleDialogCutscenePlay(DialogCutscenePlayEvent evt)
        {
            Debug.Log("DialogCutscenePlay");
            dialogSender.selectedEvent = evt.Dialogue;
            SceneManager.LoadScene("DialogCutscene");
        }
    }
}