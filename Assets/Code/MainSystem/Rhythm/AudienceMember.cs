using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.Rhythm
{
    public enum AudienceState
    {
        Idle,
        Walk,
        Cheer
    }

    public class AudienceMember : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _jumpPower = 0.5f;

        [Header("Animation Settings")]
        [SerializeField] private string _paramIdle = "IDLE";
        [SerializeField] private string _paramWalk = "MOVE";
        [SerializeField] private string _paramCheer = "CHEER";
        
        [Header("Cheer Settings")]
        [SerializeField] private float _cheerJumpPower = 0.3f;
        [SerializeField] private float _cheerIntervalMin = 0.5f;
        [SerializeField] private float _cheerIntervalMax = 1.5f;

        private int _hashIdle;
        private int _hashWalk;
        private int _hashCheer;

        private bool _isMoving;
        private bool _shouldBeCheering;
        private bool _isCheeringLoop;
        private AudienceState _currentState;

        private void Awake()
        {
            _hashIdle = Animator.StringToHash(_paramIdle);
            _hashWalk = Animator.StringToHash(_paramWalk);
            _hashCheer = Animator.StringToHash(_paramCheer);
        }

        public void Initialize(Sprite sprite)
        {
            if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();

            _hashIdle = Animator.StringToHash(_paramIdle);
            _hashWalk = Animator.StringToHash(_paramWalk);
            _hashCheer = Animator.StringToHash(_paramCheer);

            if (sprite != null && _renderer != null) _renderer.sprite = sprite;
            
            _isMoving = false;
            _shouldBeCheering = false;
            _currentState = AudienceState.Idle;
            StopCheerLoop();
            ChangeState(AudienceState.Idle);
        }

        public void ChangeState(AudienceState newState)
        {
            if (_animator == null) return;
            if (_currentState == newState && _animator.GetBool(_hashIdle) == (newState == AudienceState.Idle)) 
            {
                // Extra check to ensure animator is in sync with our state
                if (newState == AudienceState.Cheer && !_isCheeringLoop) StartCheerLoop().Forget();
                return;
            }

            _currentState = newState;

            _animator.SetBool(_hashIdle, newState == AudienceState.Idle);
            _animator.SetBool(_hashWalk, newState == AudienceState.Walk);
            _animator.SetBool(_hashCheer, newState == AudienceState.Cheer);

            if (newState == AudienceState.Cheer)
            {
                if (!_isCheeringLoop) StartCheerLoop().Forget();
            }
            else
            {
                StopCheerLoop();
            }
        }

        public async UniTaskVoid MoveToSeatAsync(Vector3 targetPos)
        {
            StopCheerLoop();
            _isMoving = true;
            ChangeState(AudienceState.Walk);

            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / _moveSpeed;

            var moveTween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
            var jumpTween = transform.DOJump(targetPos, _jumpPower, (int)(distance * 2), duration);

            await UniTask.WhenAll(
                WaitForTween(moveTween),
                WaitForTween(jumpTween)
            );

            _isMoving = false;
            SetCheeringState(_shouldBeCheering);
        }

        public void SetCheeringState(bool state)
        {
            _shouldBeCheering = state;
            if (_isMoving) return;

            ChangeState(state ? AudienceState.Cheer : AudienceState.Idle);
        }

        private async UniTaskVoid StartCheerLoop()
        {
            _isCheeringLoop = true;

            // Random initial delay to desync audience
            await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0f, 1f)));

            while (_isCheeringLoop && this != null)
            {
                float duration = 0.4f;
                await transform.DOJump(transform.position, _cheerJumpPower, 1, duration)
                               .SetEase(Ease.OutQuad)
                               .AsyncWaitForCompletion();
                
                if (!_isCheeringLoop) break;

                float interval = Random.Range(_cheerIntervalMin, _cheerIntervalMax);
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval));
            }
        }

        private void StopCheerLoop()
        {
            _isCheeringLoop = false;
            transform.DOKill(true); // Complete active tweens immediately so they land
        }

        private UniTask WaitForTween(Tween tween)
        {
            var tcs = new UniTaskCompletionSource();
            tween.OnComplete(() => tcs.TrySetResult())
                 .OnKill(() => tcs.TrySetResult());
            return tcs.Task;
        }
    }
}