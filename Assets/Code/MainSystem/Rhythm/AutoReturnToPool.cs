using UnityEngine;
using System;

namespace Code.MainSystem.Rhythm
{
    public class AutoReturnToPool : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleCompo;
        public Action<GameObject> OnReturn;

        private void OnEnable()
        {
            particleCompo.Play(true);
        }

        private void Update()
        {
            if (particleCompo.isStopped)
            {
                OnReturn?.Invoke(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
}