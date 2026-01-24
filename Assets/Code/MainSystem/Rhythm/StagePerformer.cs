using UnityEngine;
using DG.Tweening;

namespace Code.MainSystem.Rhythm
{
    public enum PerformerState
    {
        Idle,
        Hit,
        Miss
    }

    public class StagePerformer : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer; 

        [Header("Settings")]
        [SerializeField] private int _assignedTrackIndex; 
        
        [Header("Animation Settings")]
        [SerializeField] private string _paramIdle = "IDLE";
        [SerializeField] private string _paramHit = "HIT";
        [SerializeField] private string _paramMiss = "MISS";

        private int _hashIdle;
        private int _hashHit;
        private int _hashMiss;
        
        private PerformerState _currentState;

        public int TrackIndex => _assignedTrackIndex;

        public void Initialize(int trackIndex)
        {
            _assignedTrackIndex = trackIndex;
            ChangeState(PerformerState.Idle);
        }

        private void Awake()
        {
            _hashIdle = Animator.StringToHash(_paramIdle);
            _hashHit = Animator.StringToHash(_paramHit);
            _hashMiss = Animator.StringToHash(_paramMiss);
        }

        private void Start()
        {
            ChangeState(PerformerState.Idle);
        }

        public void ChangeState(PerformerState newState)
        {
            _currentState = newState;
            
            if (_animator != null)
            {
                _animator.SetBool(_hashIdle, newState == PerformerState.Idle);
                _animator.SetBool(_hashHit, newState == PerformerState.Hit);
                _animator.SetBool(_hashMiss, newState == PerformerState.Miss);
            }
        }

        public void ReactToJudgement(JudgementType judgement)
        {
            if (_animator == null) return;

            if (judgement == JudgementType.Miss)
            {
                ChangeState(PerformerState.Miss);
                
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.DOColor(Color.red, 0.1f)
                        .SetLoops(2, LoopType.Yoyo)
                        .OnComplete(() => _spriteRenderer.color = Color.white);
                }
            }
            else 
            {
                ChangeState(PerformerState.Hit);
            }
        }
    }
}