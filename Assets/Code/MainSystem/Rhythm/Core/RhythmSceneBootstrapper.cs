using UnityEngine;
using Reflex.Attributes;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Code.Core.Addressable;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.BaseStats;

using Code.MainSystem.Rhythm.Notes;
using Code.MainSystem.Rhythm.Audio;
using Code.MainSystem.Rhythm.Judgement;
using Code.MainSystem.Rhythm.UI;
using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.Rhythm.Stage;

namespace Code.MainSystem.Rhythm.Core
{
    public class RhythmSceneBootstrapper : MonoBehaviour
    {
        [Header("Scene References")]
        [Inject] private RhythmLineController _lineController;
        [Inject] private Conductor _conductor;
        [Inject] private ScoreManager _scoreManager;
        [Inject] private ChartLoader _chartLoader;
        [Inject] private HitFeedbackManager _hitFeedbackManager;
        [Inject] private JudgementSystem _judgementSystem;
        [Inject] private FeverManager _feverManager; 
        
        [SerializeField] private RhythmGameDataSenderSO _dataSender;
        [SerializeField] private CanvasGroup _loadingCanvasGroup;
        [SerializeField] private float _fadeDuration = 0.5f;

        private const string KEY_ENV_BUSKING = "RhythmGame/Environment/Busking";
        private const string KEY_ENV_LIVE    = "RhythmGame/Environment/Live";
        private const string KEY_NOTE_BASIC  = "RhythmGame/Prefab/Note_Basic";
        private const string KEY_VFX_HIT     = "RhythmGame/Prefab/HitEffect";

        private AudioClip _loadedMusic;
        private GameObject _loadedNotePrefab;
        private GameObject _loadedHitEffect;
        private GameObject _loadedEnvPrefab;
        
        private StatManager _statManager;

        private async void Start()
        {
            Time.timeScale = 1.0f;
            
            _statManager = FindAnyObjectByType<StatManager>();
            if (_statManager == null) Debug.LogWarning("[Bootstrapper] StatManager not found. Stats will not affect gameplay.");

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

            ApplyStatsToManagers();

            await FadeInGameScreen();

            if (_conductor != null)
            {
                _conductor.Play();
            }
        }

        private void ApplyStatsToManagers()
        {
            if (_statManager == null || _dataSender.members == null) return;

            if (_feverManager != null)
            {
                var teamStat = _statManager.GetTeamStat(StatType.TeamHarmony);
                float bonus = teamStat != null ? teamStat.CurrentValue * 0.1f : 0f;
                _feverManager.SetStatBonusDuration(bonus);
            }

            List<MemberType> flatMembers = new List<MemberType>();
            
            int memberIdCounter = 1;
            
             _scoreManager.SetMemberStatMultiplier(0, 1.0f);

            foreach (var group in _dataSender.members)
            {
                if (group == null || group.Members == null) continue;
                foreach (var memberType in group.Members)
                {
                    StatType targetStat = GetStatTypeForMember(memberType);
                    var statObj = _statManager.GetMemberStat(memberType, targetStat);
                    
                    float multiplier = 1.0f;
                    if (statObj != null)
                    {
                        multiplier = 1.0f + (statObj.CurrentValue * 0.001f);
                    }
                    
                    _scoreManager.SetMemberStatMultiplier(memberIdCounter, multiplier);
                    memberIdCounter++;
                    
                    break; 
                }
            }
        }

        private StatType GetStatTypeForMember(MemberType type)
        {
            switch (type)
            {
                case MemberType.Guitar: return StatType.GuitarConcentration;
                case MemberType.Bass: return StatType.BassSenseOfRhythm;
                case MemberType.Drums: return StatType.DrumsSenseOfRhythm;
                case MemberType.Piano: return StatType.PianoDexterity;
                case MemberType.Vocal: return StatType.VocalVocalization;
                default: return StatType.Condition;
            }
        }

        private async UniTask LoadGameResources()
        {
            Debug.Log($"[Bootstrapper] _dataSender.members is {(_dataSender.members == null ? "NULL" : "NOT NULL")}, Count: {(_dataSender.members != null ? _dataSender.members.Count : 0)}");

            List<Code.MainSystem.StatSystem.Manager.MemberType> memberRoles = new List<Code.MainSystem.StatSystem.Manager.MemberType>();
            if (_dataSender.members != null && _dataSender.members.Count > 0)
            {
                foreach (var memberGroup in _dataSender.members)
                {
                    if (memberGroup != null && memberGroup.Members != null)
                    {
                        foreach (var type in memberGroup.Members)
                        {
                            memberRoles.Add(type);
                            Debug.Log($"[Bootstrapper] Added Member: {type}");
                            break;
                        }
                    }
                }
            }

            // Fallback: 데이터가 없으면 기본 5인조 구성으로 강제 설정
            if (memberRoles.Count == 0)
            {
                Debug.LogWarning("[Bootstrapper] No members found in DataSender. Using Default Full Band Setup.");
                memberRoles.Add(Code.MainSystem.StatSystem.Manager.MemberType.Vocal);
                memberRoles.Add(Code.MainSystem.StatSystem.Manager.MemberType.Guitar);
                memberRoles.Add(Code.MainSystem.StatSystem.Manager.MemberType.Bass);
                memberRoles.Add(Code.MainSystem.StatSystem.Manager.MemberType.Drums);
                memberRoles.Add(Code.MainSystem.StatSystem.Manager.MemberType.Piano);
            }
            
            Debug.Log($"[Bootstrapper] Final memberRoles count: {memberRoles.Count}");

            string songId = _dataSender.SongId;
            string musicKey = $"RhythmGame/Music/{songId}";
            string envKey = _dataSender.ConcertType == ConcertType.Live ? KEY_ENV_LIVE : KEY_ENV_BUSKING;

            _loadedMusic = await SafeLoadAssetAsync<AudioClip>(musicKey);
            _loadedNotePrefab = await SafeLoadAssetAsync<GameObject>(KEY_NOTE_BASIC);
            
            _loadedHitEffect = await SafeLoadAssetAsync<GameObject>(KEY_VFX_HIT);
            _loadedEnvPrefab = await SafeLoadAssetAsync<GameObject>(envKey);

            var chartData = await ConcertChartBuilder.BuildAsync(_chartLoader, songId, memberRoles);

            if (_loadedMusic != null) _conductor.SetMusic(_loadedMusic);
            else Debug.LogError($"[Rhythm] Music is missing: {musicKey}");

            if (_loadedNotePrefab != null) 
            {
                var pulseComp = _loadedNotePrefab.GetComponent<RhythmPulse>();
                if (pulseComp != null)
                {
                    _lineController.SetPulsePrefab(pulseComp);
                }
                else
                {
                    Debug.LogError($"[Rhythm] Loaded Note Prefab does not have RhythmPulse component: {KEY_NOTE_BASIC}");
                }
            }
            else Debug.LogError($"[Rhythm] Note Prefab is missing: {KEY_NOTE_BASIC}");

            if (_loadedHitEffect != null) _hitFeedbackManager.SetHitEffectPrefab(_loadedHitEffect);
            else Debug.LogWarning($"[Rhythm] Hit Effect is missing: {KEY_VFX_HIT}");

            if (_loadedEnvPrefab != null)
            {
                GameObject envObj = Instantiate(_loadedEnvPrefab);
                var stageController = envObj.GetComponentInChildren<BuskingStageController>();
                
                if (stageController != null)
                {
                    Debug.Log($"[Bootstrapper] Found BuskingStageController. Initializing with {memberRoles.Count} members.");
                    await stageController.InitializeStage(memberRoles);
                }
                else
                {
                    Debug.LogError("[Bootstrapper] BuskingStageController component NOT found on instantiated environment!");
                }
            }
            else Debug.LogWarning($"[Rhythm] Environment Prefab is missing: {envKey}");

            if (chartData != null) _lineController.SetChart(chartData);
            else Debug.LogError($"[Rhythm] Chart Data failed to build for {songId}");
        }

        private async UniTask<T> SafeLoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key)) return null;
            
            try
            {
                return await GameResourceManager.Instance.LoadAssetAsync<T>(key).AsUniTask();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[Rhythm] Failed to load asset with key: {key}. Error: {e.Message}");
                return null;
            }
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