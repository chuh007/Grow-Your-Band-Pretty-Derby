using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Reflex.Attributes;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;

namespace Code.MainSystem.Rhythm
{
    public class MobileInputController : MonoBehaviour
    {
        [SerializeField] private int laneCount = 4;
        
        [Header("Feedback Settings")]
        [SerializeField] private List<Image> laneImages;
        [SerializeField] private float flashAlpha = 0.8f;
        [SerializeField] private float normalAlpha = 0.4f;
        [SerializeField] private float fadeSpeed = 0.1f;

        [Inject] private JudgementSystem _judgementSystem;

        private float _laneWidth;

        private void Start()
        {
            _laneWidth = Screen.width / (float)laneCount;
            
            foreach(var img in laneImages)
            {
                if(img != null) 
                {
                    var color = img.color;
                    color.a = normalAlpha;
                    img.color = color;
                }
            }
        }

        private void Update()
        {
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        Vector2 pos = touch.position.ReadValue();
                        int laneIndex = CalculateLaneIndex(pos.x);
                        HandleInput(laneIndex);
                    }
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleInput(0);
            }
        }

        private void HandleInput(int laneIndex)
        {
            // 리듬 닥터 스타일: 어떤 키/터치 입력이든 메인 라인(0번) 판정 시도
            if (_judgementSystem != null)
            {
                _judgementSystem.OnInputDetected(0);
            }
            
            // 시각적 피드백과 이벤트는 입력된 위치 그대로 전달 (UI 연출용)
            if (laneIndex >= 0 && laneIndex < laneCount)
            {
                Bus<TouchEvent>.Raise(new TouchEvent(laneIndex));
                
                if (laneImages != null && laneIndex < laneImages.Count && laneImages[laneIndex] != null)
                {
                    var img = laneImages[laneIndex];
                    img.CrossFadeAlpha(flashAlpha, 0.0f, true); 
                    img.CrossFadeAlpha(normalAlpha, fadeSpeed, true);
                }
            }
        }

        private int CalculateLaneIndex(float screenX)
        {
            if (_laneWidth <= 0) return 0; 
            int index = (int)(screenX / _laneWidth);
            return Mathf.Clamp(index, 0, laneCount - 1);
        }
    }
}