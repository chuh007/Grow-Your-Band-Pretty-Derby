using System;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    /// <summary>
    /// 리듬 게임이 전송한 데이터 읽어옴
    /// </summary>
    public class RhythmGameResultReceiver : MonoBehaviour
    {
        // 인터페이스로 뺄까
        [SerializeField] private RhythmGameDataSenderSO dataSender;

        // 일단은 Awake에서 대부분 등록하니 Start에서. 나중에 데이터 로드하는 시점 생기면 거기서
        // 올스텟 조금 상승. 하모니 상승
        private void Start()
        {
            Debug.Assert(dataSender != null, "RhythmGameDataSenderSO is missing");
            Bus<TeamStatValueChangedEvent>.Raise(new TeamStatValueChangedEvent
                (StatType.TeamHarmony,dataSender.harmonyStatUpValue));

            #region 올스텟 올리기

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

            #endregion
            
            // SO니까 초기화하기
            dataSender.members.Clear();
            dataSender.allStatUpValue = 0;
            dataSender.harmonyStatUpValue = 0;
        }
    }
}