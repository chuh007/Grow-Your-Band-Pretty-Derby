using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code.MainSystem.Rhythm.Stage
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
        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private Animator animator;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float jumpPower = 0.5f;

        [Header("Animation Settings")]
        [SerializeField] private string paramIdle = "IDLE";
        [SerializeField] private string paramWalk = "MOVE";
        [SerializeField] private string paramCheer = "CHEER";
        
        [Header("Cheer Settings")]
        [SerializeField] private float cheerJumpPower = 0.3f;
        [SerializeField] private float cheerIntervalMin = 0.5f;
        [SerializeField] private float cheerIntervalMax = 1.5f;

        private int _hashIdle;
        private int _hashWalk;
        private int _hashCheer;

        private bool _isMoving;
        private bool _shouldBeCheering;
        private bool _isCheeringLoop;
        private AudienceState _currentState;

        private void Awake()
        {
            _hashIdle = Animator.StringToHash(paramIdle);
            _hashWalk = Animator.StringToHash(paramWalk);
            _hashCheer = Animator.StringToHash(paramCheer);
        }

        public void Initialize(Sprite sprite)
        {
            if (renderer == null) renderer = GetComponentInChildren<SpriteRenderer>();
            if (animator == null) animator = GetComponentInChildren<Animator>();

            _hashIdle = Animator.StringToHash(paramIdle);
            _hashWalk = Animator.StringToHash(paramWalk);
            _hashCheer = Animator.StringToHash(paramCheer);

            if (sprite != null && renderer != null) renderer.sprite = sprite;
            
            _isMoving = false;
            _shouldBeCheering = false;
            _currentState = AudienceState.Idle;
            StopCheerLoop();
            ChangeState(AudienceState.Idle);
        }

        public void ChangeState(AudienceState newState)
        {
            if (animator == null) return;
            if (_currentState == newState && animator.GetBool(_hashIdle) == (newState == AudienceState.Idle)) 
            {
                if (newState == AudienceState.Cheer && !_isCheeringLoop) StartCheerLoop(this.GetCancellationTokenOnDestroy()).Forget();
                return;
            }

            _currentState = newState;

            animator.SetBool(_hashIdle, newState == AudienceState.Idle);
            animator.SetBool(_hashWalk, newState == AudienceState.Walk);
            animator.SetBool(_hashCheer, newState == AudienceState.Cheer);

            if (newState == AudienceState.Cheer)
            {
                if (!_isCheeringLoop) StartCheerLoop(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else
            {
                StopCheerLoop();
            }
        }

        public async UniTaskVoid MoveToSeatAsync(Vector3 targetPos)
        {
            var token = this.GetCancellationTokenOnDestroy();
            StopCheerLoop();
            _isMoving = true;
            ChangeState(AudienceState.Walk);

            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            var moveTween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
            var jumpTween = transform.DOJump(targetPos, jumpPower, (int)(distance * 2), duration);

            await UniTask.WhenAll(
                WaitForTween(moveTween, token),
                WaitForTween(jumpTween, token)
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

        private async UniTaskVoid StartCheerLoop(System.Threading.CancellationToken token)
        {
            _isCheeringLoop = true;

            await UniTask.Delay(System.TimeSpan.FromSeconds(Random.Range(0f, 1f)), cancellationToken: token);

            while (_isCheeringLoop && this != null)
            {
                float duration = 0.4f;
                await transform.DOJump(transform.position, cheerJumpPower, 1, duration)
                               .SetEase(Ease.OutQuad)
                               .AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
                
                if (!_isCheeringLoop) break;

                float interval = Random.Range(cheerIntervalMin, cheerIntervalMax);
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: token);
            }
        }

        private void StopCheerLoop()
        {
            _isCheeringLoop = false;
            transform.DOKill(true); 
        }

        private UniTask WaitForTween(Tween tween, System.Threading.CancellationToken token)
        {
            return tween.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
        }
    }
}