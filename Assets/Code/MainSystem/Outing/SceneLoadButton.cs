using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    [RequireComponent(typeof(Button))]
    public class SceneLoadButton : MonoBehaviour
    {
        public void SceneLoad(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// 지금 있는 씬을 파괴하지 않고 다른 씬을 로딩함.
        /// </summary>
        /// <param name="sceneName">이 이름을 가진 씬이 빌드 세팅에 있어야 함</param>
        public void SceneLoadAdditive(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}