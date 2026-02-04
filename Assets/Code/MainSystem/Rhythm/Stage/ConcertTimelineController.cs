using UnityEngine;
using UnityEngine.Playables;
using Code.MainSystem.Rhythm.Audio;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;
using Reflex.Attributes;
using UnityEngine.Serialization;

namespace Code.MainSystem.Rhythm.Stage
{
    public class ConcertTimelineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [Inject] private Conductor _conductor;
        private bool _isPlaying = false;

        private void Awake()
        {
            if (director != null)
            {
                director.timeUpdateMode = DirectorUpdateMode.Manual;
                director.playOnAwake = false;
            }
        }

        private void Update()
        {
            if (!_isPlaying || director == null) return;

            double currentSongTime = _conductor.SongPosition;

            director.time = currentSongTime;

            director.Evaluate();
        }

        private void OnEnable()
        {
            Bus<SongStartEvent>.OnEvent += OnSongStart;
            Bus<SongEndEvent>.OnEvent += OnSongEnd;
        }

        private void OnDisable()
        {
            Bus<SongStartEvent>.OnEvent -= OnSongStart;
            Bus<SongEndEvent>.OnEvent -= OnSongEnd;
        }

        private void OnSongStart(SongStartEvent evt)
        {
            if (director != null)
            {
                director.time = 0;
                director.Evaluate();
            }
            _isPlaying = true;
        }

        private void OnSongEnd(SongEndEvent evt)
        {
            _isPlaying = false;
        }
    }
}