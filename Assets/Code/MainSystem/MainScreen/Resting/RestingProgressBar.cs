using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Resting
{
    public class RestingProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform fillBar;

        private float _duration;
        private float _elapsed;
        private bool _isPlaying;
        private Action _onComplete;

        public async UniTask Play(float duration, Action onComplete = null)
        {
            _duration = duration;
            _elapsed = 0f;
            _isPlaying = true;
            _onComplete = onComplete;

            SetScaleX(0f);

            while (_elapsed < _duration)
            {
                await UniTask.Yield();
                _elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(_elapsed / _duration);

                SetScaleX(t);
            }

            SetScaleX(1f);

            _isPlaying = false;
            _onComplete?.Invoke();
        }

        private void SetScaleX(float x)
        {
            if (fillBar != null)
            {
                Vector3 scale = fillBar.localScale;
                scale.x = x;
                fillBar.localScale = scale;
            }
        }

        public void ResetBar()
        {
            _isPlaying = false;
            _elapsed = 0f;
            SetScaleX(0f);
        }
    }
}