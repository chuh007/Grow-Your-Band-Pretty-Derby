using UnityEngine;
using Reflex.Attributes;

namespace Code.MainSystem.Rhythm
{
    public class MobileInputController : MonoBehaviour
    {
        [SerializeField] private int laneCount = 4;

        [Inject] private JudgementSystem _judgementSystem;

        private float _laneWidth;

        private void Start()
        {
            _laneWidth = Screen.width / (float)laneCount;
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
                        if (_judgementSystem != null)
                        {
                            _judgementSystem.OnInputDetected(laneIndex);
                        }
                    }
                }
            }
            
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (_judgementSystem != null && UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.dKey.wasPressedThisFrame) _judgementSystem.OnInputDetected(0);
                if (kb.fKey.wasPressedThisFrame) _judgementSystem.OnInputDetected(1);
                if (kb.jKey.wasPressedThisFrame) _judgementSystem.OnInputDetected(2);
                if (kb.kKey.wasPressedThisFrame) _judgementSystem.OnInputDetected(3);
            }
            #endif
        }

        private int CalculateLaneIndex(float screenX)
        {
            if (_laneWidth <= 0) return 0; 
            int index = (int)(screenX / _laneWidth);
            return Mathf.Clamp(index, 0, laneCount - 1);
        }
    }
}