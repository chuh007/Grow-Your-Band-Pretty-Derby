using UnityEngine;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public class HitFeedbackManager : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Scene References")]
        [SerializeField] private Transform[] laneTransforms; 
        
        [Header("Settings")]
        [SerializeField] private Vector3 effectScale = Vector3.one;
        [SerializeField] private float effectYOffset = 100f;

        private GameObject _hitEffectPrefab;
        private Queue<GameObject> _effectPool = new Queue<GameObject>();

        private void OnEnable()
        {
            Bus<NoteHitEvent>.OnEvent += HandleNoteHit;
            Bus<TouchEvent>.OnEvent += HandleTouchEvent;
        }

        private void OnDisable()
        {
            Bus<NoteHitEvent>.OnEvent -= HandleNoteHit;
            Bus<TouchEvent>.OnEvent -= HandleTouchEvent;
        }


        public void SetHitEffectPrefab(GameObject prefab)
        {
            _hitEffectPrefab = prefab;
        }

        private void HandleNoteHit(NoteHitEvent evt)
        {
            if (evt.Judgement != JudgementType.Miss)
            {
                PlayHitSound();
                PlayHitEffect(evt.LaneIndex);
            }
        }
        private void HandleTouchEvent(TouchEvent evt)
        {
            PlayHitEffect(evt.LaneIndex);
        }

        private void PlayHitSound()
        {
            if (sfxSource != null && hitSound != null)
            {
                sfxSource.PlayOneShot(hitSound);
            }
        }

        private void PlayHitEffect(int laneIndex)
        {
            if (_hitEffectPrefab == null) return;
            if (laneTransforms == null || laneIndex < 0 || laneIndex >= laneTransforms.Length) return;

            Transform targetTransform = laneTransforms[laneIndex];
            GameObject effectInstance = GetEffectInstance();
            
            if (effectInstance != null)
            {
                effectInstance.transform.SetParent(targetTransform, false);

                // UI 앞으로
                effectInstance.transform.localPosition = new Vector3(0, effectYOffset, -500f);
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

            GameObject newInstance = Instantiate(_hitEffectPrefab);
            var returner = newInstance.GetComponent<AutoReturnToPool>();
            if (returner == null)
            {
                returner = newInstance.AddComponent<AutoReturnToPool>();
            }
            
            returner.OnReturn = (obj) => _effectPool.Enqueue(obj);
            return newInstance;
        }
    }
}