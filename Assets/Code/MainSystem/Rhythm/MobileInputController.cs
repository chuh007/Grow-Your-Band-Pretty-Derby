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
            if (_judgementSystem != null)
            {
                if (Input.GetKeyDown(KeyCode.D)) _judgementSystem.OnInputDetected(0);
                if (Input.GetKeyDown(KeyCode.F)) _judgementSystem.OnInputDetected(1);
                if (Input.GetKeyDown(KeyCode.J)) _judgementSystem.OnInputDetected(2);
                if (Input.GetKeyDown(KeyCode.K)) _judgementSystem.OnInputDetected(3);
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