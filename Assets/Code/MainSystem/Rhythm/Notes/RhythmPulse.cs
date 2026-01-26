using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Code.MainSystem.Rhythm.Audio;

namespace Code.MainSystem.Rhythm.Notes
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
        private int _fixedSteps;
        
        private Conductor _conductor;
        private bool _isInitialized = false;
        private bool _isHitPulse = false;

        public double TargetTime => _targetTime;
        public bool IsHitPulse => _isHitPulse;

        public void Initialize(Conductor conductor, Vector3 start, Vector3 end, double spawnTime, double targetTime, bool isHitPulse, int steps)
        {
            _conductor = conductor;
            _startPos = start;
            _endPos = end;
            _spawnTime = spawnTime;
            _targetTime = targetTime;
            _duration = targetTime - spawnTime;
            _isHitPulse = isHitPulse;
            _fixedSteps = Mathf.Max(1, steps);
            
            transform.position = _startPos;
            UpdateVisuals();
            
            _isInitialized = true;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isInitialized || _conductor == null) return;

            double currentSongTime = _conductor.SongPosition;
            double secPerBeat = _conductor.SecPerBeat;
            
            float t = (float)((currentSongTime - _spawnTime) / _duration);

            if (t >= 1.0f)
            {
                if (currentSongTime > _targetTime + 0.2f)
                {
                    gameObject.SetActive(false); 
                }
                else
                {
                    transform.position = _endPos;
                }
            }
            else
            {
                int steps = _fixedSteps;

                float totalProgress = t * steps; 
                int currentStepIndex = Mathf.FloorToInt(totalProgress);
                float stepProgress = totalProgress - currentStepIndex;

                float easedStepProgress = DOVirtual.EasedValue(0f, 1f, stepProgress, Ease.InExpo);

                // 최종 위치 비율 재계산
                float finalT = (currentStepIndex + easedStepProgress) / steps;

                transform.position = Vector3.LerpUnclamped(_startPos, _endPos, finalT);
            }
        }

        private void UpdateVisuals()
        {
            if (_visualImage == null) return;

            if (_isHitPulse)
            {
                transform.localScale = Vector3.one * 1.5f;
                _visualImage.color = new Color(1f, 1f, 1f, 1f); 
                if (_canvasGroup) _canvasGroup.alpha = 1f;
            }
            else
            {
                transform.localScale = Vector3.one * 0.6f;
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