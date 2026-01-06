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
        private GameObject _loadedPrefab;

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

            var musicTask = GameResourceManager.Instance.LoadAsync<AudioClip>(musicKey);
            var prefabTask = GameResourceManager.Instance.LoadAsync<GameObject>(prefabKey);
            var chartTask = ConcertChartBuilder.BuildAsync(_chartLoader, songId, memberRoles);

            await UniTask.WhenAll(musicTask.AsUniTask(), prefabTask.AsUniTask(), chartTask);

            _loadedMusic = await musicTask;
            _loadedPrefab = await prefabTask;
            var loadedChart = await chartTask;

            if (_conductor != null)
            {
                _conductor.SetAudioClip(_loadedMusic); 
            }

            if (_noteManager != null)
            {
                _noteManager.SetNotePrefab(_loadedPrefab);
                _noteManager.SetChart(loadedChart);
            }

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
                
                if (_loadedPrefab != null)
                {
                    GameResourceManager.Instance.Release(_loadedPrefab);
                    _loadedPrefab = null;
                }
            }
        }
    }
}
