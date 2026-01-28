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
            dataSender.isResultDataAvailable = false;

            if (evt.Members != null && evt.Members.Count > 0)
            {
                dataSender.members = evt.Members;
            }
            else
            {
                Debug.LogError("RhythmGameResultReceiver: No members provided in event. Cannot proceed with concert.");
                return;
            }

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
            var (isAvailable, allStatUpValue, harmonyStatUpValue) = dataSender.ConsumeResult();
            
            if (!isAvailable) return;

            Bus<TeamStatValueChangedEvent>.Raise(new TeamStatValueChangedEvent
                (StatType.TeamHarmony, harmonyStatUpValue));

            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Guitar, StatType.GuitarEndurance, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Guitar, StatType.GuitarConcentration, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Drums, StatType.DrumsSenseOfRhythm, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Drums, StatType.DrumsPower, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Bass, StatType.BassDexterity, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Bass, StatType.BassSenseOfRhythm, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Vocal, StatType.VocalVocalization, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Vocal, StatType.VocalBreathing, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Piano, StatType.PianoDexterity, allStatUpValue));
            Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                (MemberType.Piano, StatType.PianoStagePresence, allStatUpValue));
            for (int i = 0; i < (int)MemberType.Team; ++i)
            {
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    ((MemberType)i, StatType.Condition, allStatUpValue));
                Bus<StatIncreaseEvent>.Raise(new StatIncreaseEvent
                    ((MemberType)i, StatType.Mental, allStatUpValue));
            }
        }


    }
}