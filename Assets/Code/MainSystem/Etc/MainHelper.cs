using System;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.Etc
{
    /// <summary>
    /// MainScreen 참조 편하게 하기 위한 컴포넌트
    /// </summary>
    public class MainHelper : MonoBehaviour
    {
        public static MainHelper Instance;

        public MainScreen.MainScreen MainScreen { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                MainScreen = FindAnyObjectByType<MainScreen.MainScreen>();
            }
            else
            {
                Destroy(this);
            }
        }
        
        
    }
}