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
        [Inject] private ChartLoader _chartLoader;
        
        [SerializeField] private RhythmGameDataSenderSO _dataSender;

        private void Start()
        {
            if (_dataSender == null)
            {
                Debug.LogWarning("Bootstrapper: RhythmGameDataSenderSO is not assigned. Running in Test Mode or Failed.");
                return; 
            }

            Debug.Log($"Bootstrapper: Initializing Session for Song {_dataSender.SongId}");

            // 1. DataSenderSO에서 멤버 역할(Role) 추출
            // members는 List<IEnumerable<MemberType>> 구조이므로, 각 요소의 첫 번째 항목을 역할로 간주합니다.
            List<Code.MainSystem.StatSystem.Manager.MemberType> memberRoles = new List<Code.MainSystem.StatSystem.Manager.MemberType>();
            
            if (_dataSender.members != null)
            {
                foreach (var memberGroup in _dataSender.members)
                {
                    // IEnumerable<MemberType>에서 첫 번째 유효한 타입을 가져옴
                    if (memberGroup != null)
                    {
                        foreach (var type in memberGroup)
                        {
                            memberRoles.Add(type);
                            break; // 첫 번째만 가져오고 다음 멤버로 넘어감
                        }
                    }
                }
            }

            // 2. 빌더를 사용하여 메인 채보 + 멤버 채보 병합
            if (_noteManager != null && _chartLoader != null)
            {
                var finalChart = ConcertChartBuilder.Build(_chartLoader, _dataSender.SongId, memberRoles);
                _noteManager.SetChart(finalChart);
            }
            else
            {
                Debug.LogError("Bootstrapper: NoteManager or ChartLoader is missing!");
            }

            // 3. 오디오 클립 로드 및 할당
            if (_conductor != null)
            {
                // 경로: Assets/Resources/Audio/{SongId}
                string audioPath = $"Audio/{_dataSender.SongId}";
                AudioClip songClip = Resources.Load<AudioClip>(audioPath);

                if (songClip != null)
                {
                    _conductor.SetAudioClip(songClip);
                    Debug.Log($"Bootstrapper: Loaded Audio '{audioPath}'");
                }
                else
                {
                    Debug.LogError($"Bootstrapper: Audio not found at 'Resources/{audioPath}'");
                }
            }
        }
    }
}
