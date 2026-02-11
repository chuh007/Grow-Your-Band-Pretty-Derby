using System;
using System.Collections;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Etc
{
    public class ScreenRotationManager : MonoBehaviour
    {
        public static ScreenRotationManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnEnable()
        {
            Bus<ScreenRotationEvent>.OnEvent += HandleScreenRoataion;
        }

        private void HandleScreenRoataion(ScreenRotationEvent evt)
        {
            if (evt.Isverticalscreen)
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }
            else
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            Canvas.ForceUpdateCanvases();
    
            // 또는 한 프레임 대기 후 레이아웃 재계산
            StartCoroutine(RefreshUILayout());
        }

        private IEnumerator RefreshUILayout()
        {
            yield return null; // 한 프레임 대기
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<RectTransform>());
        }

        public void OnDisable()
        {
            Bus<ScreenRotationEvent>.OnEvent -= HandleScreenRoataion;
        }
    }
}