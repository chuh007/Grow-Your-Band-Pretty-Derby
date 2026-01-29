using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Code.MainSystem.Rhythm.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.SceneTransition
{
    public class RhythmGameTransitionManager : MonoBehaviour
    {
        [SerializeField] private RhythmGameDataSenderSO dataSender;
        [SerializeField] private SceneTransitionSenderSO transitionSender;
        [SerializeField] private string sceneName = "Rhythm";
        [SerializeField] private string transitionSceneName = "TransitionScene";

        private void OnEnable()
        {
            Bus<ConcertStartRequested>.OnEvent += HandleConcertStart;
        }

        private void OnDisable()
        {
            Bus<ConcertStartRequested>.OnEvent -= HandleConcertStart;
        }

        private void HandleConcertStart(ConcertStartRequested evt)
        {
            Debug.Log($"[TransitionManager] 리듬게임 시작 요청 수신: {evt.SongId}");
            
            if (dataSender != null)
            {
                dataSender.Initialize(evt.SongId, evt.ConcertType, evt.Members);
                Debug.Log("[TransitionManager] DataSender Initialized.");
            }
            else
            {
                Debug.LogError("[TransitionManager] DataSender is missing!");
            }
            transitionSender.SetTransition(sceneName, TransitionMode.ToLandscape);
            SceneManager.LoadScene(transitionSceneName);
        }
    }
}
