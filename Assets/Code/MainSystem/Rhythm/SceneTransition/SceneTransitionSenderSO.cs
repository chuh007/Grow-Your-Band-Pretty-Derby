using UnityEngine;

namespace Code.MainSystem.Rhythm.SceneTransition
{
    public enum TransitionMode
    {
        ToLandscape, 
        ToPortrait 
    }

    [CreateAssetMenu(fileName = "SceneTransitionSender", menuName = "SO/SceneTransitionSender")]
    public class SceneTransitionSenderSO : ScriptableObject
    {
        public string nextSceneName; 
        public TransitionMode mode; 

        public void SetTransition(string nextScene, TransitionMode transitionMode)
        {
            nextSceneName = nextScene;
            mode = transitionMode;
        }
    }
}
