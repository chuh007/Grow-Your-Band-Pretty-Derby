using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.MainSystem.Rhythm.Core;
using Code.MainSystem.Turn;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Code.MainSystem.Encounter
{
    /// <summary>
    /// 인카운터를 모아서 가지고 있고, 인카운터의 발생을 컨트롤함
    /// 보니깐 나중에 외출쪽도 싱글턴으로 해야겄네
    /// </summary>
    public class EncounterManager : MonoBehaviour, ITurnStartComponent
    {
        [SerializeField] private CurrentEncounterListSO currentEncounterList;
        [SerializeField] private EncounterSenderSO encounterSender;
        
        [SerializeField] private RhythmGameDataSenderSO rhythmGameDataSender;
        
        public static EncounterManager Instance;
        
        private Dictionary<EncounterConditionType, List<EncounterDataSO>> encounterData;
        private void Awake()
        {

            
            encounterData = new Dictionary<EncounterConditionType, List<EncounterDataSO>>();
            
            foreach (EncounterConditionType type in Enum.GetValues(typeof(EncounterConditionType)))
            {
                encounterData.Add(type, new List<EncounterDataSO>());
            }
            
            foreach (var encounter in currentEncounterList.encounters)
            {
                encounterData[encounter.type].Add(encounter);
            }
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                encounterSender.encounterData = null;
                encounterSender.addedTraits.Clear();
                encounterSender.changeStats.Clear();
                //rhythmGameDataSender.IsSuccess = false;
                //rhythmGameDataSender.IsFailed = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Bus<EncounterCheckEvent>.OnEvent += HandleEncounterCheck;
        }

        private void OnDestroy()
        {
            Bus<EncounterCheckEvent>.OnEvent -= HandleEncounterCheck;
        }

        private void HandleEncounterCheck(EncounterCheckEvent evt)
        {
            if (evt.Type == EncounterConditionType.BuskingCaseFall)
            {
                var data = encounterData[EncounterConditionType.BuskingCaseFall];

                encounterSender.encounterData = data[0];
                DOVirtual.DelayedCall(0.5f, 
                    () => SceneManager.LoadScene("EncounterScene", LoadSceneMode.Additive));
            }
        }

        public void TurnStart()
        {
            if (rhythmGameDataSender.IsSuccess)
            {
                var data = encounterData[EncounterConditionType.BuskingSuccess];
                encounterSender.encounterData = data[0];
                DOVirtual.DelayedCall(0.5f, 
                    () => SceneManager.LoadScene("EncounterScene", LoadSceneMode.Additive));
                return;
            }
            if(rhythmGameDataSender.IsFailed)
            {
                var data = encounterData[EncounterConditionType.BuskingFall];
                encounterSender.encounterData = data[0];
                DOVirtual.DelayedCall(0.5f, 
                    () => SceneManager.LoadScene("EncounterScene", LoadSceneMode.Additive));
                return;
            }
            
            // foreach (var data in encounterData[EncounterConditionType.TurnStart])
            // {
            //     if (Random.Range(0f, 1.0f) <= data.percent)
            //     {
            //         encounterSender.encounterData = data;
            //         encounterData[EncounterConditionType.TurnStart].Remove(data);
            //         DOVirtual.DelayedCall(0.5f, 
            //             () => SceneManager.LoadScene("EncounterScene", LoadSceneMode.Additive));
            //         return;
            //     }
            // }
        }
    }
}