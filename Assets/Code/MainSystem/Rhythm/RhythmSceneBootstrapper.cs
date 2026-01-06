using UnityEngine;
using Reflex.Attributes;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Code.Core.Addressable;

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

        private AudioClip _loadedMusic;
        private GameObject _loadedNotePrefab;

        private async void Start()
        {
            if (_dataSender == null)
            {
                Debug.LogWarning("Bootstrapper: RhythmGameDataSenderSO is not assigned. Running in Test Mode or Failed.");
                return; 
            }

            Debug.Log($"Bootstrapper: Initializing Session for Song {_dataSender.SongId}");

            List<Code.MainSystem.StatSystem.Manager.MemberType> memberRoles = new List<Code.MainSystem.StatSystem.Manager.MemberType>();
            if (_dataSender.members != null)
            {
                foreach (var memberGroup in _dataSender.members)
                {
                    if (memberGroup != null)
                    {
                        foreach (var type in memberGroup)
                        {
                            memberRoles.Add(type);
                            break;
                        }
                    }
                }
            }

            string songId = _dataSender.SongId;
            string musicKey = $"RhythmGame/Music/{songId}";
            string prefabKey = "RhythmGame/Prefab/Note";

            var musicLoadTask = GameResourceManager.Instance.LoadAssetAsync<AudioClip>(musicKey).AsUniTask();
            var prefabLoadTask = GameResourceManager.Instance.LoadAssetAsync<GameObject>(prefabKey).AsUniTask();
            var chartLoadTask = ConcertChartBuilder.BuildAsync(_chartLoader, songId, memberRoles);

            var (musicClip, notePrefab, chartData) = await UniTask.WhenAll(
                musicLoadTask, 
                prefabLoadTask, 
                chartLoadTask
            );

            if (musicClip != null) 
            {
                _loadedMusic = musicClip;
                _conductor.SetMusic(musicClip);
            }
            
            if (notePrefab != null)
            {
                _loadedNotePrefab = notePrefab;
                _noteManager.SetNotePrefab(notePrefab);
            }

            _noteManager.SetChart(chartData);

            if (_conductor != null)
            {
                _conductor.Play();
            }
        }

        private void OnDestroy()
        {
            if (GameResourceManager.Instance != null)
            {
                if (_loadedMusic != null)
                {
                    GameResourceManager.Instance.Release(_loadedMusic);
                    _loadedMusic = null;
                }
                
                if (_loadedNotePrefab != null)
                {
                    GameResourceManager.Instance.Release(_loadedNotePrefab);
                    _loadedNotePrefab = null;
                }
            }
        }
    }
}
