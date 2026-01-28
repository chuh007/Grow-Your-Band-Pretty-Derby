#if UNITY_EDITOR
using UnityEngine;

namespace Code.MainSystem.Rhythm.ChartEditor.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class EditorAudioController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip metronomeTick;
        
        [Header("Settings")]
        [SerializeField] private bool metronomeEnabled = false;
        
        private double _currentBPM = 120.0;
        private double _lastBeatTime = -1.0;
        
        public bool IsPlaying => audioSource != null && audioSource.isPlaying;
        public double CurrentTime => audioSource != null ? audioSource.time : 0.0;
        public float Duration => (audioSource != null && audioSource.clip != null) ? audioSource.clip.length : 0f;
        public AudioClip CurrentClip => audioSource != null ? audioSource.clip : null;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (IsPlaying && metronomeEnabled)
            {
                HandleMetronome();
            }
        }

        private void HandleMetronome()
        {
            double secPerBeat = 60.0 / _currentBPM;
            double currentTime = audioSource.time;
            
            int lastBeatIndex = Mathf.FloorToInt((float)(_lastBeatTime / secPerBeat));
            int currentBeatIndex = Mathf.FloorToInt((float)(currentTime / secPerBeat));

            if (currentBeatIndex > lastBeatIndex)
            {
                 PlayMetronomeTick();
            }
            
            _lastBeatTime = currentTime;
        }

        private void PlayMetronomeTick()
        {
             if(metronomeTick != null) audioSource.PlayOneShot(metronomeTick);
        }

        public void Play()
        {
            if (audioSource != null)
            {
                audioSource.Play();
                _lastBeatTime = audioSource.time;
            }
        }

        public void Pause()
        {
            if (audioSource != null) audioSource.Pause();
        }

        public void Seek(double time)
        {
            if (audioSource != null)
            {
                audioSource.time = Mathf.Clamp((float)time, 0f, audioSource.clip != null ? audioSource.clip.length : 0f);
                _lastBeatTime = audioSource.time;
            }
        }
        
        public void SeekStep(float direction)
        {
             double step = (60.0 / _currentBPM) / 32.0;
             Seek(audioSource.time + (step * direction));
        }

        public void SetPlaybackSpeed(float speed)
        {
            if (audioSource != null) audioSource.pitch = speed;
        }

        public void SetBPM(double bpm)
        {
            _currentBPM = bpm;
        }
        
        public void SetClip(AudioClip clip)
        {
            if (audioSource != null) audioSource.clip = clip;
        }
        
        public void ToggleMetronome(bool isOn)
        {
            metronomeEnabled = isOn;
        }
    }
}

#endif