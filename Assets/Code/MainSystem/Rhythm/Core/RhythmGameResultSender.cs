using System;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.Rhythm.SceneTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.Core
{
    /// <summary>
    /// 리듬 게임에서 메인 씬으로 전송시 사용
    /// </summary>
    public class RhythmGameResultSender : MonoBehaviour
    {
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        [SerializeField] private SceneTransitionSenderSO transitionSender;
        
        private RhythmGameResultEvent? _cachedResult;
        
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
            _cachedResult = evt;
        }

        public void SubmitResultAndExit()
        {
            if (_cachedResult == null)
            {
                Debug.LogWarning("No result data to send!");
                if (transitionSender != null)
                {
                    //transitionSender.SetTransition("Lobby", TransitionMode.ToPortrait);
                    transitionSender.SetTransition("Test", TransitionMode.ToPortrait);
                    SceneManager.LoadScene("TransitionScene");
                }
                else
                {
                    //SceneManager.LoadScene("Lobby");
                    SceneManager.LoadScene("Test");
                }
                return;
            }

            var evt = _cachedResult.Value;

            // 이거 랭크가 이넘이면 편하겄는데
            // 지금은 임시로 해둔다
            dataSender.allStatUpValue = 1 + (int)(evt.FinalScore * 0.1f);
            int memberCount = dataSender.MemberIds != null ? dataSender.MemberIds.Count : 0;
            dataSender.harmonyStatUpValue = memberCount * (int)(evt.FinalScore * 0.1f);
            
            dataSender.IsResultDataAvailable = true;

            if (transitionSender != null)
            {
                //transitionSender.SetTransition("Lobby", TransitionMode.ToPortrait);
                transitionSender.SetTransition("Test", TransitionMode.ToPortrait);
                SceneManager.LoadScene("TransitionScene");
            }
            else
            {
                Debug.Log("Fail to submit result");
                //SceneManager.LoadScene("Lobby");
                SceneManager.LoadScene("Test");
            }
        }
    }
}