using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour, IMusicController
    {
        private AudioSource _audioSource;

        public bool IsPlaying => _audioSource != null && _audioSource.isPlaying;

        public double ClipLength => _audioSource.clip != null ? _audioSource.clip.length : 0;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            if (_audioSource.clip != null)
            {
                _audioSource.Play();
            }
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public double GetCurrentTime()
        {
            if (_audioSource.clip == null) return 0;
            return (double)_audioSource.time;
        }

        public void SetAudioClip(AudioClip clip)
        {
            _audioSource.clip = clip;
        }
    }
}