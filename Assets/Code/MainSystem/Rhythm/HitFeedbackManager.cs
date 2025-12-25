using UnityEngine;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public class HitFeedbackManager : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private ParticleSystem hitEffectPrefab;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Scene References")]
        [SerializeField] private Transform[] laneTransforms; 

        private void Start()
        {
            Bus<ScoreUpdateEvent>.OnEvent += HandleScoreUpdate;
        }

        private void OnDestroy()
        {
            Bus<ScoreUpdateEvent>.OnEvent -= HandleScoreUpdate;
        }

        private void HandleScoreUpdate(ScoreUpdateEvent evt)
        {
            if (evt.LastJudgement != JudgementType.Miss)
            {
                PlayHitSound();
                PlayHitEffect(evt.LaneIndex);
            }
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
            if (hitEffectPrefab == null) return;
            if (laneTransforms == null || laneIndex < 0 || laneIndex >= laneTransforms.Length) return;

            Transform targetTransform = laneTransforms[laneIndex];
            ParticleSystem effect = Instantiate(hitEffectPrefab, targetTransform.position, Quaternion.identity);
            effect.Play();
            
            Destroy(effect.gameObject, 1.0f);
        }
    }
}