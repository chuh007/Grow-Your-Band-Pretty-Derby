using UnityEngine;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public abstract class StageController : MonoBehaviour
    {
        [SerializeField] protected float targetScore = 100000f;

        protected virtual void OnEnable()
        {
            Bus<ScoreUpdateEvent>.OnEvent += OnScoreUpdated;
        }

        protected virtual void OnDisable()
        {
            Bus<ScoreUpdateEvent>.OnEvent -= OnScoreUpdated;
        }

        private void OnScoreUpdated(ScoreUpdateEvent evt)
        {
            float progress = Mathf.Clamp01(evt.CurrentScore / targetScore);
            OnProgressUpdated(progress, evt.CurrentScore);
        }

        protected abstract void OnProgressUpdated(float progress, float currentScore);
    }
}
