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
        [Inject] private HitFeedbackManager _hitFeedbackManager;
        
        [SerializeField] private RhythmGameDataSenderSO _dataSender;
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private float _fadeDuration = 0.5f;

        // 배경
        private const string KEY_ENV_BUSKING = "RhythmGame/Environment/Busking";
        private const string KEY_ENV_LIVE    = "RhythmGame/Environment/Live";
        // 노트 및 이펙트
        private const string KEY_NOTE_BASIC  = "RhythmGame/Prefab/Note_Basic";
        private const string KEY_VFX_HIT     = "RhythmGame/Prefab/HitEffect";

        private AudioClip _loadedMusic;
        private GameObject _loadedNotePrefab;
        private GameObject _loadedHitEffect;
        private GameObject _loadedEnvPrefab;

        private async void Start()
        {
            if (_loadingCanvasGroup != null)
            {
                _loadingCanvasGroup.alpha = 1f;
                _loadingCanvasGroup.gameObject.SetActive(true);
                _loadingCanvasGroup.blocksRaycasts = true;
            }

            if (_dataSender == null)
            {
                Debug.LogWarning("Bootstrapper: RhythmGameDataSenderSO is not assigned. Running in Test Mode or Failed.");
                return; 
            }

            Debug.Log($"Bootstrapper: Initializing Session for Song {_dataSender.SongId}");

            await LoadGameResources();

            await FadeInGameScreen();

            if (_conductor != null)
            {
                _conductor.Play();
            }
        }

        private async UniTask LoadGameResources()
        {
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
            
            string envKey = _dataSender.ConcertType == ConcertType.Live ? KEY_ENV_LIVE : KEY_ENV_BUSKING;

            var musicLoadTask = GameResourceManager.Instance.LoadAssetAsync<AudioClip>(musicKey).AsUniTask();
            var prefabLoadTask = GameResourceManager.Instance.LoadAssetAsync<GameObject>(KEY_NOTE_BASIC).AsUniTask();
            var chartLoadTask = ConcertChartBuilder.BuildAsync(_chartLoader, songId, memberRoles);
            var vfxLoadTask = GameResourceManager.Instance.LoadAssetAsync<GameObject>(KEY_VFX_HIT).AsUniTask();
            var envLoadTask = GameResourceManager.Instance.LoadAssetAsync<GameObject>(envKey).AsUniTask();

            var (musicClip, notePrefab, chartData, hitEffect, envPrefab) = await UniTask.WhenAll(
                musicLoadTask, 
                prefabLoadTask, 
                chartLoadTask,
                vfxLoadTask,
                envLoadTask
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
            
            if (hitEffect != null)
            {
                _loadedHitEffect = hitEffect;
                _hitFeedbackManager.SetHitEffectPrefab(hitEffect);
            }

            if (envPrefab != null)
            {
                _loadedEnvPrefab = envPrefab;
                Instantiate(envPrefab);
            }

            _noteManager.SetChart(chartData);
        }

        private async UniTask FadeInGameScreen()
        {
            if (_loadingCanvasGroup == null) return;

            float elapsed = 0f;
            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                _loadingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
                await UniTask.Yield();
            }

            _loadingCanvasGroup.gameObject.SetActive(false);
            _loadingCanvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            Screen.orientation = ScreenOrientation.Portrait;

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

                if (_loadedHitEffect != null)
                {
                    GameResourceManager.Instance.Release(_loadedHitEffect);
                    _loadedHitEffect = null;
                }

                if (_loadedEnvPrefab != null)
                {
                    GameResourceManager.Instance.Release(_loadedEnvPrefab);
                    _loadedEnvPrefab = null;
                }
            }
        }
    }
}
