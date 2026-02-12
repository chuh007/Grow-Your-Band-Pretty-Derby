using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Code.Core.Bus;
using Code.MainSystem.Rhythm.Data;
using Code.Core.Bus.GameEvents.RhythmEvents;

namespace Code.MainSystem.Rhythm.Notes
{
    public class RhythmLineVisualizer : MonoBehaviour
    {
        [Header("Line Settings")]
        [SerializeField] private Image lineImage;
        [SerializeField] private float scrollSpeed = 1.0f;

        [Header("Punch Settings (Note Hit)")]
        [SerializeField] private RectTransform punchTarget;
        [SerializeField] private float punchScale = 1.2f;
        [SerializeField] private float punchDuration = 0.15f;

        [Header("Guideline Pulse Settings (Beat)")]
        [SerializeField] private GameObject guidelinePrefab;
        [SerializeField] private Transform container;
        [SerializeField] private float pulseDuration = 0.12f;
        
        [SerializeField] private AnimationCurve pulseCurveStrong = new AnimationCurve(
            new Keyframe(0f, 0.3f), 
            new Keyframe(0.2f, 1f), 
            new Keyframe(1f, 0f)
        );
        
        [SerializeField] private AnimationCurve pulseCurveWeak = new AnimationCurve(
            new Keyframe(0f, 0.3f), 
            new Keyframe(0.2f, 1f), 
            new Keyframe(1f, 0f)
        );

        private List<Transform> _guidelines = new List<Transform>();
        
        public void Initialize(int beatCount)
        {
            foreach (var g in _guidelines)
            {
                if (g != null) Destroy(g.gameObject);
            }
            _guidelines.Clear();

            if (container != null && container.childCount > 0)
            {
                for (int i = container.childCount - 1; i >= 0; i--)
                {
                    Destroy(container.GetChild(i).gameObject);
                }
            }

            if (guidelinePrefab == null || container == null)
            {
                Debug.LogWarning("[RhythmLineVisualizer] GuidelinePrefab or Container is not assigned.");
                return;
            }

            for (int i = 0; i < beatCount; i++)
            {
                GameObject go = Instantiate(guidelinePrefab, container);
                _guidelines.Add(go.transform);
            }
            
            Debug.Log($"[RhythmLineVisualizer] Initialized with {beatCount} guidelines.");
        }

        private void Start()
        {
            Bus<NoteHitEvent>.OnEvent += HandleNoteHit;
            Bus<BeatPulseEvent>.OnEvent += HandleBeatPulse;
        }

        private void OnDestroy()
        {
            Bus<NoteHitEvent>.OnEvent -= HandleNoteHit;
            Bus<BeatPulseEvent>.OnEvent -= HandleBeatPulse;
        }

        private void Update()
        {
            float offset = Time.time * scrollSpeed;
            Vector2 textureOffset = new Vector2(-offset, 0);

            if (lineImage != null && lineImage.material != null)
            {
                lineImage.material.mainTextureOffset = textureOffset;
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

        private void HandleBeatPulse(BeatPulseEvent evt)
        {
            if (_guidelines == null || _guidelines.Count == 0) return;

            bool isStrongBeat = evt.BeatIndex % RhythmGameBalanceConsts.BEAT_INTERVAL_STRONG == 0;
            float targetScale = isStrongBeat ? RhythmGameBalanceConsts.PULSE_SCALE_STRONG : RhythmGameBalanceConsts.PULSE_SCALE_WEAK;
            AnimationCurve targetCurve = isStrongBeat ? pulseCurveStrong : pulseCurveWeak;

            // 현재 비트에 해당하는 보조선 하나만 움찔하게
            int index = evt.BeatIndex % _guidelines.Count;
            if (index >= 0 && index < _guidelines.Count)
            {
                var guideline = _guidelines[index];
                if (guideline != null)
                {
                    PulseRoutine(guideline, targetScale, targetCurve).Forget();
                }
            }
        }

        private async UniTaskVoid PulseRoutine(Transform target, float targetMaxScale, AnimationCurve curve)
        {
            if (target == null) return;

            Vector3 originalScale = Vector3.one;
            float elapsed = 0f;

            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pulseDuration;
                float curveValue = curve.Evaluate(t);
                
                target.localScale = originalScale * (1f + (targetMaxScale - 1f) * curveValue);
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            }

            if (target != null)
            {
                target.localScale = originalScale;
            }
        }

        private IEnumerator PunchRoutine()
        {
            if (punchTarget == null) yield break;

            Vector3 originalScale = Vector3.one;
            Vector3 targetScale = Vector3.one * punchScale;
            
            float elapsed = 0f;
            while (elapsed < punchDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (punchDuration / 2);
                punchTarget.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < punchDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (punchDuration / 2);
                punchTarget.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            punchTarget.localScale = originalScale;
        }
    }
}