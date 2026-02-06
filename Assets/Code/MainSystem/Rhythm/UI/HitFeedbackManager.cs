using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.RhythmEvents;

using Code.MainSystem.Rhythm.Data;
using Unity.Cinemachine;

namespace Code.MainSystem.Rhythm.UI
{
    public class HitFeedbackManager : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Scene References")]
        [SerializeField] private Transform hitEffectAnchor;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        
        [Header("Settings")]
        [SerializeField] private Vector3 effectScale = Vector3.one;
        [SerializeField] private float effectYOffset = 1.0f;

        private GameObject _hitEffectPrefab;
        private Queue<GameObject> _effectPool = new Queue<GameObject>();
        private int _lastEffectFrame = -1;

        private void Awake()
        {
            if (hitEffectAnchor == null)
            {
                Debug.LogWarning("[HitFeedbackManager] hitEffectAnchor is not assigned! Using self as fallback.");
                hitEffectAnchor = this.transform;
            }
            
            if (impulseSource == null)
            {
                impulseSource = GetComponent<CinemachineImpulseSource>();
            }
        }

        private void OnEnable()
        {
            Bus<NoteHitEvent>.OnEvent += HandleNoteHit;
        }

        private void OnDisable()
        {
            Bus<NoteHitEvent>.OnEvent -= HandleNoteHit;
        }


        public void SetHitEffectPrefab(GameObject prefab)
        {
            _hitEffectPrefab = prefab;
            Debug.Log($"[HitFeedbackManager] Prefab set: {(_hitEffectPrefab != null ? _hitEffectPrefab.name : "NULL")}");
        }

        private void HandleNoteHit(NoteHitEvent evt)
        {
            if (evt.Judgement != JudgementType.Miss)
            {
                PlayHitSound();
                PlayHitEffect();
                
                if (evt.Judgement == JudgementType.Perfect)
                {
                    TriggerCameraShake();
                }
            }
        }

        private void TriggerCameraShake()
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(Vector3.down * RhythmGameBalanceConsts.IMPULSE_FORCE_PERFECT);
            }
        }

        private void PlayHitSound()
        {
            if (sfxSource != null && hitSound != null)
            {
                sfxSource.PlayOneShot(hitSound);
            }
        }

        private void PlayHitEffect()
        {
            if (Time.frameCount == _lastEffectFrame) return;
            _lastEffectFrame = Time.frameCount;

            if (_hitEffectPrefab == null || hitEffectAnchor == null) return;

            GameObject effectInstance = GetEffectInstance();
            
            if (effectInstance != null)
            {
                if (effectInstance.transform.parent != hitEffectAnchor)
                {
                    effectInstance.transform.SetParent(hitEffectAnchor, false);
                }

                effectInstance.transform.localPosition = new Vector3(0, effectYOffset, -10f);
                effectInstance.transform.localRotation = Quaternion.identity;
                effectInstance.transform.localScale = effectScale;

                effectInstance.SetActive(true);
            }
        }

        private GameObject GetEffectInstance()
        {
            if (_effectPool.Count > 0)
            {
                return _effectPool.Dequeue();
            }

            GameObject newInstance = Instantiate(_hitEffectPrefab, hitEffectAnchor);
            var returner = newInstance.GetComponent<AutoReturnToPool>();
            
            if (returner == null)
            {
                Debug.LogError($"[HitFeedbackManager] AutoReturnToPool component missing on {_hitEffectPrefab.name}. Please attach it in Editor. Returning null to avoid runtime allocation errors.");
                Destroy(newInstance);
                return null;
            }
            
            returner.onReturn = (obj) => _effectPool.Enqueue(obj);
            return newInstance;
        }
    }
}