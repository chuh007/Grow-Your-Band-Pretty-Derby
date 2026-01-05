using UnityEngine;
using UnityEngine.UI;
using Reflex.Attributes;
using System.Collections.Generic;

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
            
            // 초기 알파값 설정
            foreach(var img in laneImages)
            {
                if(img != null) 
                {
                    var color = img.color;
                    color.a = normalAlpha;
                    img.color = color;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                HandleInput(i);
            }
        }

        private void Update()
        {
            // 터치 입력 처리 (New Input System의 Touchscreen 사용 권장되나, 
            // 프로젝트 설정이 'Both'라면 기존 Input.touchCount도 동작함)
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.phase == TouchPhase.Began)
                    {
                        int laneIndex = CalculateLaneIndex(touch.position.x);
                        HandleInput(laneIndex);
                    }
                }
            }
            
            // 에디터/키보드 입력 처리 (New Input System 방식)
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (_judgementSystem != null && UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.dKey.wasPressedThisFrame) HandleInput(0);
                if (kb.fKey.wasPressedThisFrame) HandleInput(1);
                if (kb.jKey.wasPressedThisFrame) HandleInput(2);
                if (kb.kKey.wasPressedThisFrame) HandleInput(3);
            }
            #endif
        }

        private void HandleInput(int laneIndex)
        {
            if (laneIndex < 0 || laneIndex >= laneCount) return;

            // 1. 판정 요청
            if (_judgementSystem != null)
            {
                _judgementSystem.OnInputDetected(laneIndex);
            }

            // 2. 시각적 피드백
            if (laneImages != null && laneIndex < laneImages.Count && laneImages[laneIndex] != null)
            {
                var img = laneImages[laneIndex];
                // 즉시 밝게
                img.CrossFadeAlpha(flashAlpha, 0.0f, true); 
                // 서서히 어둡게
                img.CrossFadeAlpha(normalAlpha, fadeSpeed, true);
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