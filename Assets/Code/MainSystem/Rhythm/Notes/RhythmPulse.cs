using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Code.MainSystem.Rhythm.Audio;

namespace Code.MainSystem.Rhythm.Notes
{
    public class RhythmPulse : MonoBehaviour
    {
        [SerializeField] private Image visualImage;
        [SerializeField] private CanvasGroup canvasGroup;

        public double TargetTime => _targetTime;
        
        private Vector3 _startPos;
        private Vector3 _endPos;
        private double _spawnTime;
        private double _targetTime;
        private double _duration;
        private int _fixedSteps;
        
        private Conductor _conductor;
        private bool _isInitialized = false;
        private bool _isHitPulse = false;

        private void Awake()
        {
            if (visualImage == null) visualImage = GetComponentInChildren<Image>();
            if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        public void Initialize(Conductor conductor, Vector3 start, Vector3 end, double spawnTime, double targetTime, bool isHitPulse, int steps)
        {
            _conductor = conductor;
            _startPos = start;
            _endPos = end;
            _startPos.z = 0;
            _endPos.z = 0;
            
            _spawnTime = spawnTime;
            _targetTime = targetTime;
            _duration = targetTime - spawnTime;
            _isHitPulse = isHitPulse;
            _fixedSteps = Mathf.Max(1, steps);
            
            transform.localPosition = _startPos;
            
            _isInitialized = true;
            gameObject.SetActive(true);
            
            if (visualImage != null)
            {
                visualImage.gameObject.SetActive(true);
                visualImage.enabled = true;
                
                // [안전장치] URP에서 기본 UI 머티리얼이 깨지는 현상 방지
                if (visualImage.material == null || visualImage.material.name.Contains("Default"))
                {
                    visualImage.material = new Material(Shader.Find("UI/Default"));
                }
            }
            
            UpdateVisuals();
        }

        private void Update()
        {
            if (!_isInitialized || _conductor == null) return;

            double currentSongTime = _conductor.SongPosition;
            
            if (_duration <= 0)
            {
                transform.localPosition = _endPos;
                if (currentSongTime > _targetTime + 0.5f) gameObject.SetActive(false);
                return;
            }

            float t = Mathf.Clamp01((float)((currentSongTime - _spawnTime) / _duration));

            if (t >= 1.0f)
            {
                if (currentSongTime > _targetTime + 0.5f)
                {
                    gameObject.SetActive(false); 
                }
                else
                {
                    transform.localPosition = _endPos;
                }
            }
            else
            {
                int steps = _fixedSteps;
                float totalProgress = t * steps; 
                int currentStepIndex = Mathf.FloorToInt(totalProgress);
                float stepProgress = totalProgress - currentStepIndex;
                float easedStepProgress = DOVirtual.EasedValue(0f, 1f, stepProgress, Ease.InExpo);
                float finalT = (currentStepIndex + easedStepProgress) / steps;

                Vector3 targetPos = Vector3.LerpUnclamped(_startPos, _endPos, finalT);
                targetPos.z = 0;
                transform.localPosition = targetPos;
            }
            
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (visualImage == null) return;

            // 자식 오브젝트(Visual) 트랜스폼 유지
            visualImage.rectTransform.localScale = Vector3.one;
            visualImage.rectTransform.localPosition = Vector3.zero;
            
            // 프리팹에서 설정한 크기를 유지하고 싶다면 아래 라인을 주석 처리하거나 적절한 크기로 조절하세요.
            // visualImage.rectTransform.sizeDelta = new Vector2(100, 100);
            
            if (visualImage.canvasRenderer != null)
            {
                visualImage.canvasRenderer.cullTransparentMesh = false;
                visualImage.canvasRenderer.SetAlpha(1f);
            }

            if (_isHitPulse)
            {
                transform.localScale = Vector3.one * 1.5f;
                visualImage.color = Color.white; 
                if (canvasGroup) canvasGroup.alpha = 1f;
            }
            else
            {
                transform.localScale = Vector3.one * 0.6f;
                visualImage.color = new Color(1f, 1f, 1f, 0.5f);
                if (canvasGroup) canvasGroup.alpha = 0.5f;
            }
        }

        public void Deactivate()
        {
            _isInitialized = false;
            gameObject.SetActive(false);
        }
    }
}