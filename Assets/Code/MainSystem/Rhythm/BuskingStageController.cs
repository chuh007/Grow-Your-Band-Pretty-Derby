using UnityEngine;
using System.Collections.Generic;
using Code.Core.Addressable;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Code.MainSystem.Rhythm
{
    public class BuskingStageController : StageController
    {
        [Header("Points")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private List<Transform> standPoints;

        [Header("Data")]
        [SerializeField] private List<Sprite> audienceSprites;
        
        private const string KEY_AUDIENCE_PREFAB = "RhythmGame/Prefab/AudienceMember";

        [Header("Settings")]
        [SerializeField] private int _maxAudienceCount = 20;
        [SerializeField] private int _millisecondsDelay = 200;

        private GameObject _audiencePrefabSource;
        private List<AudienceMember> _activeAudience = new List<AudienceMember>();
        private int _currentAudienceCount = 0;
        private bool _isSpawning = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadAudiencePrefab().Forget();
        }

        private async UniTaskVoid LoadAudiencePrefab()
        {
            _audiencePrefabSource = await GameResourceManager.Instance.LoadAssetAsync<GameObject>(KEY_AUDIENCE_PREFAB);
        }

        protected override void OnProgressUpdated(float progress)
        {
            int targetCount = Mathf.FloorToInt(progress * _maxAudienceCount);
            targetCount = Mathf.Min(targetCount, standPoints.Count);

            if (targetCount > _currentAudienceCount && !_isSpawning)
            {
                SpawnAudienceSequence(targetCount).Forget();
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
                if (_currentAudienceCount >= standPoints.Count) break;

                SpawnSingleAudience(_currentAudienceCount);
                _currentAudienceCount++;

                await UniTask.Delay(_millisecondsDelay);
            }

            _isSpawning = false;
        }

        private void SpawnSingleAudience(int index)
        {
            GameObject obj = Instantiate(_audiencePrefabSource, spawnPoint.position, Quaternion.identity, transform);
            var member = obj.GetComponent<AudienceMember>();

            // Sprite sprite = audienceSprites.Count > 0 
            //     ? audienceSprites[Random.Range(0, audienceSprites.Count)] 
            //     : null;
            
            Sprite sprite = audienceSprites[0];
            
            member.Initialize(sprite);

            Transform targetSeat = standPoints[index];
            member.MoveToSeatAsync(targetSeat.position).Forget();
            
            _activeAudience.Add(member);
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
