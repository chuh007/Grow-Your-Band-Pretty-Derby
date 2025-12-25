using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public interface IMusicController
    {
        void Play();
        void Stop();
        void Pause();
        double GetCurrentTime();
        bool IsPlaying { get; }
        double ClipLength { get; }
        void SetAudioClip(AudioClip clip);
    }
}