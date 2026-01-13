using UnityEngine;

namespace Code.MainSystem.Rhythm.SceneTransition
{
    public enum TransitionMode
    {
        ToLandscape, // 세로 -> 가로 (리겜 진입)
        ToPortrait // 가로 -> 세로 (메인씬 복귀)
    }

    [CreateAssetMenu(fileName = "SceneTransitionSender", menuName = "SO/SceneTransitionSender")]
    public class SceneTransitionSenderSO : ScriptableObject
    {
        public string NextSceneName; // 이동할 씬 이름
        public TransitionMode Mode; // 전환할 방향

        public void SetTransition(string nextScene, TransitionMode mode)
        {
            NextSceneName = nextScene;
            Mode = mode;
        }
    }
}
