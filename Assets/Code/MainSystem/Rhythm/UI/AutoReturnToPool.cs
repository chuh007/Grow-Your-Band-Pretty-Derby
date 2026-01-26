using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Code.MainSystem.Rhythm.UI
{
    public class AutoReturnToPool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleCompo;
        public Action<GameObject> onReturn;
        
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            if (particleCompo != null)
            {
                particleCompo.Play(true);
                ReturnAfterDelay(particleCompo.main.duration).Forget();
            }
        }

        private async UniTaskVoid ReturnAfterDelay(float duration)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(duration + 0.1f), cancellationToken: token);
                
                if (this != null && gameObject.activeSelf)
                {
                     onReturn?.Invoke(gameObject);
                     gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void OnDisable()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}