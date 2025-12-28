using UnityEngine;
using Reflex.Attributes;
using System.Collections.Generic;

namespace Code.MainSystem.Rhythm
{
    public class RhythmSceneBootstrapper : MonoBehaviour
    {
        [Header("Scene References")]
        [Inject] private NoteManager _noteManager;
        [Inject] private Conductor _conductor;
        [Inject] private ScoreManager _scoreManager;
        
        [SerializeField] private RhythmGameDataSenderSO _dataSender;

        private void Start()
        {
            if (_dataSender == null)
            {
                Debug.LogWarning("Bootstrapper: RhythmGameDataSenderSO is not assigned. Running in Test Mode or Failed.");
                return; 
            }

            Debug.Log($"Bootstrapper: Initializing Session for Song {_dataSender.SongId}");

            if (_noteManager != null)
            {
                _noteManager.SetChart(_dataSender.CombinedChart);
            }
        }
    }
}
