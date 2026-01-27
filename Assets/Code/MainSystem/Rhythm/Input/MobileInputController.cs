using UnityEngine;
using UnityEngine.InputSystem;
using Reflex.Attributes;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.RhythmEvents;

using Code.MainSystem.Rhythm.Judgement;

namespace Code.MainSystem.Rhythm.Input
{
    public class MobileInputController : MonoBehaviour
    {
        [Inject] private JudgementSystem _judgementSystem;

        private void Update()
        {
            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        HandleInput();
                    }
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleInput();
            }
        }

        private void HandleInput()
        {
            // 리듬 닥터 스타일: 어떤 키/터치 입력이든 판정 시도
            if (_judgementSystem != null)
            {
                _judgementSystem.OnInputDetected();
            }
            
            // 시각적 피드백과 이벤트는 -1 (Global)로 전달
            Bus<TouchEvent>.Raise(new TouchEvent(-1));
        }
    }
}