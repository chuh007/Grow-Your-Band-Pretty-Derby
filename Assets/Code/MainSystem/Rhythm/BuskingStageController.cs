using UnityEngine;
using System.Collections.Generic;
using Code.Core.Addressable;
using Cysharp.Threading.Tasks;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.Rhythm
{
    public class BuskingStageController : StageController
    {
        [Header("Points")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private List<Transform> standPoints;
        [SerializeField] private Transform _stageTransform;

        [Header("Data")]
        [SerializeField] private List<Sprite> audienceSprites;
        
        private const string KEY_AUDIENCE_PREFAB = "RhythmGame/Prefab/AudienceMember";

        [Header("Band Members")]
        [SerializeField] private List<Transform> _performerSpawnPoints;
        private List<BandMemberController> _performers = new List<BandMemberController>();

        [Header("Settings")]
        [SerializeField] private int _maxAudienceCount = 20;
        [SerializeField] private int _millisecondsDelay = 500;
        [SerializeField] private float _positionRandomness = 5.0f;
        [SerializeField] private int _comboThreshold = 5;
        [SerializeField] private int _scorePerAudience = 10000;
        [SerializeField] private int _pointCapacity = 5;
        [SerializeField] private float _personalSpaceRadius = 0.8f;
        [SerializeField] private int _spawnAttempts = 15;

        [Header("Collision Settings")]
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _collisionCheckRadius = 0.4f;
        [SerializeField] private int _maxSpawnAttempts = 10;
        
        private GameObject _audiencePrefabSource;
        private List<AudienceMember> _activeAudience = new List<AudienceMember>();
        private int _currentAudienceCount = 0;
        private bool _isSpawning = false;
        
        private int _currentCombo;
        private int _hitCount;
        private int _totalCount;
        
        private int[] _pointOccupancy;

        public async UniTask InitializeStage(List<MemberType> members)
        {
            if (members == null) 
            {
                Debug.LogError("[BuskingStage] InitializeStage called with NULL member list.");
                return;
            }

            Debug.Log($"[BuskingStage] InitializeStage Started. Members: {members.Count}, SpawnPoints: {(_performerSpawnPoints != null ? _performerSpawnPoints.Count : 0)}");

            if (_performerSpawnPoints == null || _performerSpawnPoints.Count == 0)
            {
                Debug.LogError("[BuskingStage] CRITICAL: Performer Spawn Points are missing or empty! Band members cannot be spawned. Check the inspector.");
                return;
            }

            foreach(var p in _performers) 
            { 
                if(p != null) Destroy(p.gameObject); 
            }
            _performers.Clear();

            for (int i = 0; i < members.Count; i++)
            {
                if (i >= _performerSpawnPoints.Count) 
                {
                    Debug.LogWarning($"[BuskingStage] Not enough spawn points! Member {i} skipped.");
                    break;
                }

                MemberType memberType = members[i];
                string key = $"RhythmGame/Prefab/Member/{memberType}";
                
                try 
                {
                    Debug.Log($"[BuskingStage] Loading Addressable: {key}");
                    GameObject prefab = await GameResourceManager.Instance.LoadAssetAsync<GameObject>(key);
                    if (prefab != null)
                    {
                        Transform point = _performerSpawnPoints[i];
                        GameObject obj = Instantiate(prefab, point.position, point.rotation, transform);
                        
                        BandMemberController performer = obj.GetComponent<BandMemberController>();
                        if (performer != null)
                        {
                            performer.Initialize(i + 1);
                            _performers.Add(performer);
                            Debug.Log($"[BuskingStage] Spawned {memberType} at index {i}");
                        }
                        else
                        {
                            Debug.LogError($"[BuskingStage] Prefab loaded but BandMemberController component missing on {memberType}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"[BuskingStage] Failed to load prefab for {memberType} (Key: {key}) - returned null");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[BuskingStage] Failed to load member {memberType}: {e.Message}");
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (standPoints != null)
            {
                _pointOccupancy = new int[standPoints.Count];
            }
            
            LoadAudiencePrefab().Forget();
            Bus<NoteHitEvent>.OnEvent += OnNoteHit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Bus<NoteHitEvent>.OnEvent -= OnNoteHit;
        }

        private async UniTaskVoid LoadAudiencePrefab()
        {
            _audiencePrefabSource = await GameResourceManager.Instance.LoadAssetAsync<GameObject>(KEY_AUDIENCE_PREFAB);
        }

        protected override void OnProgressUpdated(float progress, float currentScore)
        {
            int targetCount = Mathf.FloorToInt(currentScore / _scorePerAudience);
            targetCount = Mathf.Min(targetCount, _maxAudienceCount);

            if (targetCount > _currentAudienceCount && !_isSpawning)
            {
                SpawnAudienceSequence(targetCount).Forget();
            }
        }

        private void OnNoteHit(NoteHitEvent evt)
        {
            _totalCount++;

            if (evt.Judgement == JudgementType.Miss)
            {
                _currentCombo = 0;
            }
            else
            {
                _currentCombo++;
                _hitCount++;
            }

            if (_performers != null)
            {
                foreach (var performer in _performers)
                {
                    if (performer.TrackIndex == evt.TrackIndex)
                    {
                        performer.ReactToJudgement(evt.Judgement);
                        break;
                    }
                }
            }

            UpdateAudienceReaction();
        }

        private void UpdateAudienceReaction()
        {
            if (_activeAudience.Count == 0) return;

            float comboFactor = Mathf.Clamp01((float)_currentCombo / _comboThreshold);
            float currentAccuracy = _totalCount > 0 ? (float)_hitCount / _totalCount : 0f;
            float targetHype = currentAccuracy * comboFactor;
            
            int cheerCount = Mathf.FloorToInt(_activeAudience.Count * targetHype);

            for (int i = 0; i < _activeAudience.Count; i++)
            {
                _activeAudience[i].SetCheeringState(i < cheerCount);
            }
        }

        private async UniTaskVoid SpawnAudienceSequence(int targetCount)
        {
            _isSpawning = true;

            if (_audiencePrefabSource == null)
            {
                await UniTask.WaitUntil(() => _audiencePrefabSource != null);
            }

            while (_currentAudienceCount < targetCount)
            {
                SpawnSingleAudience(_currentAudienceCount);
                _currentAudienceCount++;

                await UniTask.Delay(_millisecondsDelay);
            }

            _isSpawning = false;
        }

        private void SpawnSingleAudience(int index)
        {
            if (standPoints.Count == 0) return;

            int targetIndex = GetBestStandPointIndex();
            Transform targetStandPoint = standPoints[targetIndex];
            _pointOccupancy[targetIndex]++;

            GameObject obj = Instantiate(_audiencePrefabSource, spawnPoint.position, Quaternion.identity, transform);
            var member = obj.GetComponent<AudienceMember>();

            Sprite sprite = audienceSprites.Count > 0 
                ? audienceSprites[Random.Range(0, audienceSprites.Count)] 
                : null;
            
            member.Initialize(sprite);

            Vector3 finalDestination = targetStandPoint.position;
            bool validPositionFound = false;

            for (int i = 0; i < _maxSpawnAttempts; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * _positionRandomness;
                Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
                Vector3 candidatePos = targetStandPoint.position + randomOffset;

                if (!Physics.CheckSphere(candidatePos, _collisionCheckRadius, _obstacleLayer))
                {
                    finalDestination = candidatePos;
                    validPositionFound = true;
                    break;
                }
            }

            member.MoveToSeatAsync(finalDestination).Forget();
            
            _activeAudience.Add(member);
        }

        private int GetBestStandPointIndex()
        {
            if (_pointOccupancy == null || _pointOccupancy.Length != standPoints.Count)
            {
                _pointOccupancy = new int[standPoints.Count];
            }

            int minOccupancy = int.MaxValue;
            int bestIndex = 0;
            bool foundSpot = false;

            for (int i = 0; i < standPoints.Count; i++)
            {
                if (_pointOccupancy[i] < _pointCapacity)
                {
                    if (_pointOccupancy[i] < minOccupancy)
                    {
                        minOccupancy = _pointOccupancy[i];
                        bestIndex = i;
                        foundSpot = true;
                    }
                }
            }

            if (foundSpot) return bestIndex;

            int absoluteMinIndex = 0;
            int absoluteMinVal = int.MaxValue;
            
            for (int i = 0; i < standPoints.Count; i++)
            {
                if (_pointOccupancy[i] < absoluteMinVal)
                {
                    absoluteMinVal = _pointOccupancy[i];
                    absoluteMinIndex = i;
                }
            }
            
            return absoluteMinIndex;
        }

        private void OnDestroy()
        {
            if (_audiencePrefabSource != null && GameResourceManager.Instance != null)
            {
                GameResourceManager.Instance.Release(_audiencePrefabSource);
            }
        }
    }
}