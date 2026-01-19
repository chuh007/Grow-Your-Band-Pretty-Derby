using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.Rhythm
{
    public class AudienceMember : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Animator _animator;

        [Header("Settings")]
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _jumpPower = 0.5f;
        
        private static readonly int IsWalking = Animator.StringToHash("WALK");
        private static readonly int IsCheering = Animator.StringToHash("CHEER");

        private Transform _cameraTransform;

        private void Start()
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (_cameraTransform != null)
            {
                Vector3 targetPos = transform.position + _cameraTransform.rotation * Vector3.forward;
                Vector3 targetOrientation = new Vector3(targetPos.x, transform.position.y, targetPos.z);
                transform.LookAt(targetOrientation);
            }
        }

        public void Initialize(Sprite sprite)
        {
            if (_renderer == null) _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();

            if (sprite != null && _renderer != null) _renderer.sprite = sprite;
            
            if (_animator != null)
            {
                _animator.SetBool("WALK", false);
                _animator.SetBool("CHEER", false);
            }
        }

        public async UniTaskVoid MoveToSeatAsync(Vector3 targetPos)
        {
            _animator.SetBool(IsWalking, true);

            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / _moveSpeed;

            var moveTween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
            var jumpTween = transform.DOJump(targetPos, _jumpPower, (int)(distance * 2), duration);

            await UniTask.WhenAll(
                WaitForTween(moveTween),
                WaitForTween(jumpTween)
            );

            _animator.SetBool(IsWalking, false);
            _animator.SetBool(IsCheering, true);
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