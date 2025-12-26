using UnityEngine;
using System;
using Code.Core.Bus;

namespace Code.MainSystem.Rhythm
{
    public class Conductor : MonoBehaviour
    {
        public static Conductor Instance { get; private set; }

        [Header("Dependencies")]
        [SerializeField] private GameObject musicControllerObject;
        private IMusicController _musicController;

        [Header("Settings")]
        [SerializeField] private double bpm = 120.0d;
        [SerializeField] private float inputOffset = 0.0f; 

        public double InputOffset => inputOffset;

        public event Action OnSongStart;
        public event Action OnSongEnd;
        public event Action<int> OnBeatPulse;

        public double SongPosition { get; private set; }
        public double SongPositionInBeats { get; private set; }
        public double SecPerBeat { get; private set; }
        
        private int _lastBeat = 0;
        private bool _isPlaying = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (musicControllerObject != null)
            {
                _musicController = musicControllerObject.GetComponent<IMusicController>();
            }
            else
            {
                _musicController = GetComponent<IMusicController>();
            }

            if (_musicController == null)
            {
                Debug.LogError("Conductor: No IMusicController found!");
            }

            RecalculateBpmTiming();
        }

        private void RecalculateBpmTiming()
        {
            if (bpm <= 0) bpm = 120.0d;
            SecPerBeat = 60.0d / bpm;
        }

        public void SetBpm(double newBpm)
        {
            bpm = newBpm;
            RecalculateBpmTiming();
        }

        public void Play()
        {
            if (_musicController == null) return;

            _isPlaying = true;
            _lastBeat = 0;
            _musicController.Play();
            
            OnSongStart?.Invoke();
            
            Bus<SongStartEvent>.Raise(new SongStartEvent());
        }

        public void Stop()
        {
            if (_musicController == null) return;

            _isPlaying = false;
            _musicController.Stop();
            
            OnSongEnd?.Invoke();
            Bus<SongEndEvent>.Raise(new SongEndEvent());
        }

        public void Pause()
        {
            if (_musicController == null) return;
            _isPlaying = false;
            _musicController.Pause();
        }

        private void Update()
        {
            if (!_isPlaying || _musicController == null) return;

            SongPosition = _musicController.GetCurrentTime();
            
            if (SongPosition >= _musicController.ClipLength && _musicController.ClipLength > 0)
            {
                if (!_musicController.IsPlaying) 
                {
                    Stop();
                    return;
                }
            }

            SongPositionInBeats = SongPosition / SecPerBeat;

            int currentBeat = (int)SongPositionInBeats;
            if (currentBeat > _lastBeat)
            {
                _lastBeat = currentBeat;
                
                OnBeatPulse?.Invoke(currentBeat);
                
                Bus<BeatPulseEvent>.Raise(new BeatPulseEvent(currentBeat));
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}