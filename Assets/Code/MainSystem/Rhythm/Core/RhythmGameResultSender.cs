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
                    transitionSender.SetTransition("Test", TransitionMode.ToPortrait);
                    SceneManager.LoadScene("TransitionScene");
                }
                else
                {
                    SceneManager.LoadScene("Test");
                }
                return;
            }

            var evt = _cachedResult.Value;

            dataSender.allStatUpValue = 1 + (int)(evt.FinalScore * 0.1f);
            int memberCount = dataSender.members != null ? dataSender.members.Count : 0;
            dataSender.harmonyStatUpValue = memberCount * (int)(evt.FinalScore * 0.1f);
            
            dataSender.isResultDataAvailable = true;

            if (transitionSender != null)
            {
                transitionSender.SetTransition("Test", TransitionMode.ToPortrait);
                SceneManager.LoadScene("TransitionScene");
            }
            else
            {
                Debug.Log("Fail to submit result");
                SceneManager.LoadScene("Test");
            }
        }
    }
}