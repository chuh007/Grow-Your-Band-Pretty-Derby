using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
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
        
        private void Start()
        {
            Debug.Assert(dataSender != null, "RhythmGameDataSenderSO is missing");

            // 씬 진입 시 처리해야 할 리듬 게임 결과 데이터가 있는지 확인
            if (dataSender.isResultDataAvailable)
            {
                Debug.Log("[RhythmGameResultReceiver] 결과 데이터 처리 중...");
                ProcessGameResult();
            }
            
            // 결과 처리가 완료되었거나 데이터가 없는 경우, 안전하게 SO의 모든 필드를 초기화
            dataSender.Initialize();
            Debug.Log("[RhythmGameResultReceiver] 데이터 세션 초기화 완료.");
        }

        /// <summary>
        /// SO에 저장된 리듬 게임 결과를 기반으로 멤버들의 스탯을 올리고 턴을 종료
        /// </summary>
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

            Bus<TurnEndEvent>.Raise(new TurnEndEvent());
        }

    }
}