using UnityEngine;

namespace Code.MainSystem.Rhythm.Data
{
    [CreateAssetMenu(fileName = "SongData", menuName = "SO/Rhythm/SongData")]
    public class SongDataSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string SongId;          // 리소스 로드 키
        public string Title;           // 곡 제목 (UI 표시용)
        public float Bpm;              // 곡의 템포 (Conductor 설정용)

        [Header("Rhythm Settings")]
        public int BeatCountPerBar;    // 한 마디당 박자 수 (예: 4, 7)
        public float GuidelineSpacing; // 보조선 간격 (기본값: 150f)

        [Header("Resources")]
        public AudioClip MusicClip;    // 오디오 파일
    }
}