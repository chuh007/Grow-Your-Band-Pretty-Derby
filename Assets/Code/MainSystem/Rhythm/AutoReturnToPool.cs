using UnityEngine;
using System;

namespace Code.MainSystem.Rhythm
{
    [RequireComponent(typeof(ParticleSystem))]
    public class AutoReturnToPool : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        public Action<GameObject> OnReturn;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            _particleSystem.Play(true);
        }

        private void Update()
        {
            if (_particleSystem.isStopped)
            {
                OnReturn?.Invoke(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}