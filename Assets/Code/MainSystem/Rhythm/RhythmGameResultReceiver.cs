using System;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
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
            // TODO 진호가 스텟 그냥 올리는 거 만들어줄거임
            for (int i = (int)StatType.Mental + 1; i < (int)StatType.TeamHarmony; i++)
            {
                
            }
            
            // SO니까 초기화하기
            dataSender.members.Clear();
            dataSender.allStatUpValue = 0;
            dataSender.harmonyStatUpValue = 0;
        }
    }
}