using Code.MainSystem.Rhythm.Flow;
using Code.MainSystem.Rhythm.Session;
using UnityEngine;
using Reflex.Attributes;

namespace Code.MainSystem.Rhythm
{
    public class RhythmSceneBootstrapper : MonoBehaviour
    {
        [Header("Scene References")]
        [Inject] private NoteManager _noteManager;
        [Inject] private Conductor _conductor;
        [Inject] private ScoreManager _scoreManager;
        
        private ConcertFlowCoordinator _flowCoordinator;

        private void Start()
        {
            _flowCoordinator = FindObjectOfType<ConcertFlowCoordinator>();

            if (_flowCoordinator == null || _flowCoordinator.CurrentSession == null)
            {
                Debug.LogWarning("Bootstrapper: No ConcertFlowCoordinator or Session found. Running in Test Mode.");
                return; 
            }

            ConcertSession session = _flowCoordinator.CurrentSession;

            Debug.Log($"Bootstrapper: Initializing Session for Song {session.SongId}");

            if (_noteManager != null)
            {
                _noteManager.SetChart(session.CombinedChart);
            }
        }
    }
}
