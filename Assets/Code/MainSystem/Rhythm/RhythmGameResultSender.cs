using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    /// <summary>
    /// 리듬 게임에서 메인 씬으로 전송시 사용
    /// </summary>
    public class RhythmGameResultSender : MonoBehaviour
    {
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        
        private void Awake()
        {
            Bus<RhythmGameResultEvent>.OnEvent += OnRhythmGameResult;
        }
        
        private void OnDestroy()
        {
            Bus<RhythmGameResultEvent>.OnEvent -= OnRhythmGameResult;
        }
        
        private void OnRhythmGameResult(RhythmGameResultEvent evt)
        {
            // 이거 랭크가 이넘이면 편하겄는데
            // 지금은 임시로 해둔다
            dataSender.allStatUpValue = 1 + (int)(evt.FinalScore * 0.1f);
            dataSender.harmonyStatUpValue = dataSender.members.Count * (int)(evt.FinalScore * 0.1f);
        }
    }
}