using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Rhythm
{
    public class RhythmPulse : MonoBehaviour
    {
        [SerializeField] private Image _visualImage;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Vector3 _startPos;
        private Vector3 _endPos;
        private double _spawnTime;
        private double _targetTime;
        private double _duration;
        
        private Conductor _conductor;
        private bool _isInitialized = false;
        private bool _isHitPulse = false;

        public double TargetTime => _targetTime;
        public bool IsHitPulse => _isHitPulse;

        public void Initialize(Conductor conductor, Vector3 start, Vector3 end, double spawnTime, double targetTime, bool isHitPulse)
        {
            _conductor = conductor;
            _startPos = start;
            _endPos = end;
            _spawnTime = spawnTime;
            _targetTime = targetTime;
            _duration = targetTime - spawnTime;
            _isHitPulse = isHitPulse;
            
            transform.position = _startPos;
            UpdateVisuals();
            
            _isInitialized = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isInitialized || _conductor == null) return;

            double currentSongTime = _conductor.SongPosition;
            
            float t = (float)((currentSongTime - _spawnTime) / _duration);

            if (t >= 1.0f)
            {
                // 약간 더 진행해서 자연스럽게 사라지게 함 (오버슈트 허용)
                // 만약 너무 많이 지났으면 비활성화
                if (currentSongTime > _targetTime + 0.2f)
                {
                    gameObject.SetActive(false); // 컨트롤러가 풀로 회수하도록
                }
                else
                {
                    // 1.0 넘어도 계속 이동 (Start -> End 방향으로)
                    transform.position = Vector3.LerpUnclamped(_startPos, _endPos, t);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(_startPos, _endPos, t);
            }
        }

        private void UpdateVisuals()
        {
            if (_visualImage == null) return;

            if (_isHitPulse)
            {
                transform.localScale = Vector3.one * 2.5f;
                _visualImage.color = new Color(1f, 1f, 1f, 1f); 
                if (_canvasGroup) _canvasGroup.alpha = 1f;
            }
            else
            {
                transform.localScale = Vector3.one * 0.8f;
                _visualImage.color = new Color(1f, 1f, 1f, 0.5f);
                if (_canvasGroup) _canvasGroup.alpha = 0.5f;
            }
        }

        public void Deactivate()
        {
            _isInitialized = false;
            gameObject.SetActive(false);
        }
    }
}