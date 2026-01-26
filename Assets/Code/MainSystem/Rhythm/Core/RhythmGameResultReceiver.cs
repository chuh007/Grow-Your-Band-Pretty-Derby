using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.Rhythm.SceneTransition;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.Core
{
    public class RhythmGameResultReceiver : MonoBehaviour
    {
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        [SerializeField] private SceneTransitionSenderSO transitionSender;

        private void OnEnable()
        {
            Bus<ConcertStartRequested>.OnEvent += HandleConcertStart;
        }

        private void OnDisable()
        {
            Bus<ConcertStartRequested>.OnEvent -= HandleConcertStart;
        }

        private void Start()
        {
            Debug.Assert(dataSender != null, "RhythmGameDataSenderSO is missing");

            if (dataSender.isResultDataAvailable)
            {
                ProcessGameResult();
                dataSender.isResultDataAvailable = false;
            }
        }

        private void HandleConcertStart(ConcertStartRequested evt)
        {
            dataSender.songId = evt.SongId;
            dataSender.memberIds = evt.MemberIds;
            dataSender.difficulty = evt.Difficulty;
            dataSender.isResultDataAvailable = false;
            
            dataSender.members = new List<MemberGroup>
            {
                new MemberGroup { Members = new List<MemberType> { MemberType.Vocal } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Guitar } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Bass } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Drums } },
                new MemberGroup { Members = new List<MemberType> { MemberType.Piano } }
            };

            if (transitionSender != null)
            {
                transitionSender.SetTransition("Rhythm", TransitionMode.ToLandscape);
            }
            else
            {
                Debug.LogWarning("RhythmGameResultReceiver: SceneTransitionSenderSO is missing. Defaulting to direct load or inconsistent state.");
            }

            SceneManager.LoadScene("TransitionScene");
        }

        private void ProcessGameResult()
        {
            Bus<TeamStatValueChangedEvent>.Raise(new TeamStatValueChangedEvent
                (StatType.TeamHarmony, dataSender.harmonyStatUpValue));

            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Guitar, StatType.GuitarEndurance, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Guitar, StatType.GuitarConcentration, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Drums, StatType.DrumsSenseOfRhythm, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Drums, StatType.DrumsPower, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Bass, StatType.BassDexterity, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Bass, StatType.BassSenseOfRhythm, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Vocal, StatType.VocalVocalization, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Vocal, StatType.VocalBreathing, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Piano, StatType.PianoDexterity, dataSender.allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Piano, StatType.PianoStagePresence, dataSender.allStatUpValue));
            for (int i = 0; i < (int)MemberType.Team; ++i)
            {
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    ((MemberType)i, StatType.Condition, dataSender.allStatUpValue));
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    ((MemberType)i, StatType.Mental, dataSender.allStatUpValue));
            }

            dataSender.allStatUpValue = 0;
            dataSender.harmonyStatUpValue = 0;
        }


    }
}