using UnityEngine;

namespace Code.MainSystem.Rhythm
{
    public class BandMemberController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public int TrackIndex { get; private set; }
        
        private static readonly int Miss = Animator.StringToHash("MISS");
        private static readonly int Hit = Animator.StringToHash("HIT");

        public void Initialize(int trackIndex)
        {
            TrackIndex = trackIndex;
        }

        public void ReactToJudgement(JudgementType type)
        {
            if (animator == null) return;

            if (type == JudgementType.Miss)
            {
                animator.SetTrigger(Miss);
            }
            else
            {
                animator.SetTrigger(Hit);
            }
        }
    }
}