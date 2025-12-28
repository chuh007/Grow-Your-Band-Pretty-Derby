using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.Rhythm.Session;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.Flow
{
    public class ConcertFlowCoordinator : MonoBehaviour
    {
        public static ConcertFlowCoordinator Instance { get; private set; }
        
        public ConcertSession CurrentSession { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Bus<ConcertStartRequested>.OnEvent += HandleConcertStart;
            Bus<RhythmGameResultEvent>.OnEvent += HandleGameResult;
        }

        private void OnDisable()
        {
            Bus<ConcertStartRequested>.OnEvent -= HandleConcertStart;
            Bus<RhythmGameResultEvent>.OnEvent -= HandleGameResult;
        }

        private void HandleConcertStart(ConcertStartRequested evt)
        {
            List<NoteData> combinedChart = MockLoadAndCombineChart(evt.SongId); 

            CurrentSession = new ConcertSession(evt.SongId, evt.MemberIds, evt.Difficulty, combinedChart);

            SceneManager.LoadScene("RhythmScene"); 
        }

        private void HandleGameResult(RhythmGameResultEvent evt)
        {
            if (CurrentSession != null)
            {
                CurrentSession.SetResult(evt);
            }
            
            SceneManager.LoadScene("MainScene");
        }

        private List<NoteData> MockLoadAndCombineChart(string songId)
        {
            // ChartLoader is now a MonoBehaviour injected in the scene.
            // Returning empty list to avoid compilation error.
            return new List<NoteData>(); 
        }
    }
}
