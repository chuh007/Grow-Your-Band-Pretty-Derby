using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.Rhythm;

namespace Code.MainSystem.Rhythm
{
    public class RhythmLineVisualizer : MonoBehaviour
    {
        [Header("Line Settings")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Image _lineImage;
        [SerializeField] private float _scrollSpeed = 1.0f;

        [Header("Punch Settings")]
        [SerializeField] private RectTransform _punchTarget;
        [SerializeField] private float _punchScale = 1.2f;
        [SerializeField] private float _punchDuration = 0.15f;

        private void Start()
        {
            Bus<NoteHitEvent>.OnEvent += HandleNoteHit;
        }

        private void OnDestroy()
        {
            Bus<NoteHitEvent>.OnEvent -= HandleNoteHit;
        }

        private void Update()
        {
            float offset = Time.time * _scrollSpeed;
            Vector2 textureOffset = new Vector2(-offset, 0);

            if (_lineRenderer != null && _lineRenderer.material != null)
            {
                _lineRenderer.material.mainTextureOffset = textureOffset;
            }
            
            if (_lineImage != null && _lineImage.material != null)
            {
                _lineImage.material.mainTextureOffset = textureOffset;
            }
        }

        private void HandleNoteHit(NoteHitEvent evt)
        {
            if (evt.Judgement != JudgementType.Miss)
            {
                StopAllCoroutines();
                StartCoroutine(PunchRoutine());
            }
        }

        private IEnumerator PunchRoutine()
        {
            if (_punchTarget == null) yield break;

            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = Vector3.one * _punchScale;
            
            float elapsed = 0f;
            while (elapsed < _punchDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (_punchDuration / 2);
                _punchTarget.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < _punchDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (_punchDuration / 2);
                _punchTarget.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            _punchTarget.localScale = originalScale;
        }
    }
}