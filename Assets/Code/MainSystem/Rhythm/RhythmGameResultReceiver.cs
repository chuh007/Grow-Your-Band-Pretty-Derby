using System;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm
{
    /// <summary>
    /// 리듬 게임이 전송한 데이터 읽어옴 & 게임 시작 처리
    /// </summary>
    public class RhythmGameResultReceiver : MonoBehaviour
    {
        // 인터페이스로 뺄까
        [SerializeField] private RhythmGameDataSenderSO dataSender;

        [SerializeField] private ChartLoader _chartLoader;

        private void OnEnable()
        {
            Bus<ConcertStartRequested>.OnEvent += HandleConcertStart;
        }

        private void OnDisable()
        {
            Bus<ConcertStartRequested>.OnEvent -= HandleConcertStart;
        }

        private void Awake()
        {
            if (_chartLoader == null)
            {
                _chartLoader = GetComponent<ChartLoader>();
                if (_chartLoader == null) _chartLoader = gameObject.AddComponent<ChartLoader>();
            }
        }

        // 일단은 Awake에서 대부분 등록하니 Start에서. 나중에 데이터 로드하는 시점 생기면 거기서
        // 올스텟 조금 상승. 하모니 상승
        private void Start()
        {
            Debug.Assert(dataSender != null, "RhythmGameDataSenderSO is missing");

            if (dataSender.IsResultDataAvailable)
            {
                ProcessGameResult();
                dataSender.IsResultDataAvailable = false;
            }
        }

        private void HandleConcertStart(ConcertStartRequested evt)
        {
            List<NoteData> combinedChart = LoadAndCombineChart(evt.SongId, evt.MemberIds);

            dataSender.SongId = evt.SongId;
            dataSender.MemberIds = evt.MemberIds;
            dataSender.Difficulty = evt.Difficulty;
            dataSender.CombinedChart = combinedChart;
            dataSender.IsResultDataAvailable = false;

            SceneManager.LoadScene("RhythmScene");
        }

        private List<NoteData> LoadAndCombineChart(string songId, List<int> memberIds)
        {
            List<List<NoteData>> allCharts = new List<List<NoteData>>();

            foreach (int memberId in memberIds)
            {
                string path = $"Charts/{songId}/{memberId}";
                var chart = _chartLoader.LoadChartFromResources(path);
                
                if (chart != null && chart.Count > 0)
                {
                    allCharts.Add(chart);
                }
                else
                {
                    Debug.LogWarning($"Chart not found or empty for Song: {songId}, Member: {memberId} at path: {path}");
                }
            }
            
            if (allCharts.Count == 0)
            {
                Debug.LogWarning("No charts loaded. Returning Test Chart.");
                return _chartLoader.LoadTestChart();
            }

            return _chartLoader.CombineCharts(allCharts);
        }

        private void ProcessGameResult()
        {
            Bus<TeamStatValueChangedEvent>.Raise(new TeamStatValueChangedEvent
                (StatType.TeamHarmony, dataSender.harmonyStatUpValue));

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
            if (dataSender.members != null) dataSender.members.Clear();
            dataSender.allStatUpValue = 0;
            dataSender.harmonyStatUpValue = 0;
        }


    }
}