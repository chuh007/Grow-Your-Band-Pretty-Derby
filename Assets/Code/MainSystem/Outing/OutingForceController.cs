using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.MainSystem.Outing
{
    public class OutingForceController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera camera1;
        [SerializeField] private CinemachineCamera camera2;
        [SerializeField] private CinemachineCamera camera3;
        [SerializeField] private CinemachineCamera camera4;
        [SerializeField] private CinemachineCamera camera5;
        
        private List<CinemachineCamera> _cameras;
        
        private void Awake()
        {
            _cameras = new List<CinemachineCamera>();
            _cameras.Add(camera1);
            _cameras.Add(camera2);
            _cameras.Add(camera3);
            _cameras.Add(camera4);
            _cameras.Add(camera5);
        }

        public void SetCamera(OutingPlace outingPlace)
        {
            for (int i = 0; i < _cameras.Count; i++)
            {
                if ((int)outingPlace == i)
                {
                    _cameras[i].Priority = 10;
                }
                else _cameras[i].Priority = 0;
            }
        }
    }
}