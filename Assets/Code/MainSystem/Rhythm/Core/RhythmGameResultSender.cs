using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.Rhythm.SceneTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.Core
{
    public class RhythmGameResultSender : MonoBehaviour
    {
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        [SerializeField] private SceneTransitionSenderSO transitionSender;
        [SerializeField] private string sceneName = "lch";
        [SerializeField] private string transitionSceneName = "TransitionScene";
        
        
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
                    transitionSender.SetTransition(sceneName, TransitionMode.ToPortrait);
                    SceneManager.LoadScene(transitionSceneName);
                }
                else
                {
                    SceneManager.LoadScene(sceneName);
                }
                return;
            }

            var evt = _cachedResult.Value;

            int currentScore = evt.FinalScore;
            
            bool isSuccess = currentScore >= RhythmGameConsts.SUCCESS_SCORE_THRESHOLD;
            
            dataSender.SetResult(currentScore, isSuccess);

            if (isSuccess)
            {
                dataSender.allStatUpValue = RhythmGameStatCalculator.CalculateAllStatGain(currentScore);
                int memberCount = dataSender.members != null ? dataSender.members.Count : 0;
                dataSender.harmonyStatUpValue = RhythmGameStatCalculator.CalculateHarmonyStatGain(currentScore, memberCount);
            }
            
            dataSender.isResultDataAvailable = true;

            if (transitionSender != null)
            {
                transitionSender.SetTransition(sceneName, TransitionMode.ToPortrait);
                SceneManager.LoadScene(transitionSceneName);
            }
            else
            {
                Debug.Log("Fail to submit result");
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}