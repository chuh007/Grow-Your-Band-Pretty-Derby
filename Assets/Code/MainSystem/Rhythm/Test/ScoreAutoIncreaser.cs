using UnityEngine;
using Reflex.Attributes;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.Rhythm.Test
{
    public class ScoreAutoIncreaser : MonoBehaviour
    {
        [Inject] private ScoreManager _scoreManager;

        [Header("Settings")]
        [SerializeField] private bool _autoPlay = true;
        [SerializeField] private float _startDelay = 3f;
        [SerializeField] private float _interval = 0.5f;
        [SerializeField] private JudgementType _simulateType = JudgementType.Perfect;

        private bool _isPlaying = false;

        private void Start()
        {
            if (_autoPlay)
            {
                StartAutoPlay().Forget();
            }
        }

        public void ToggleAutoPlay()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying)
            {
                StartAutoPlay().Forget();
            }
        }

        private async UniTaskVoid StartAutoPlay()
        {
            _isPlaying = true;

            if (_startDelay > 0)
            {
                Debug.Log($"[ScoreAutoIncreaser] Waiting for {_startDelay}s before starting...");
                await UniTask.Delay((int)(_startDelay * 1000));
            }

            Debug.Log("[ScoreAutoIncreaser] Start increasing score automatically...");

            while (_isPlaying && this != null)
            {
                if (_scoreManager != null)
                {
                    // Simulate a Perfect hit on a random lane (0-3)
                    _scoreManager.RegisterResult(_simulateType, Random.Range(0, 4));
                }

                await UniTask.Delay((int)(_interval * 1000));
            }
        }

        private void OnDestroy()
        {
            _isPlaying = false;
        }
    }
}
