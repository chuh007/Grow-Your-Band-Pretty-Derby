using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public class MobileInputController : MonoBehaviour
    {
        [SerializeField] private int laneCount = 4;

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
                        if (JudgementSystem.Instance != null)
                        {
                            JudgementSystem.Instance.OnInputDetected(laneIndex);
                        }
                    }
                }
            }
            
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (JudgementSystem.Instance != null)
            {
                if (Input.GetKeyDown(KeyCode.D)) JudgementSystem.Instance.OnInputDetected(0);
                if (Input.GetKeyDown(KeyCode.F)) JudgementSystem.Instance.OnInputDetected(1);
                if (Input.GetKeyDown(KeyCode.J)) JudgementSystem.Instance.OnInputDetected(2);
                if (Input.GetKeyDown(KeyCode.K)) JudgementSystem.Instance.OnInputDetected(3);
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