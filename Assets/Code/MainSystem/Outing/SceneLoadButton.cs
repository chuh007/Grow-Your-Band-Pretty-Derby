using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    [RequireComponent(typeof(Button))]
    public class SceneLoadButton : MonoBehaviour
    {
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}