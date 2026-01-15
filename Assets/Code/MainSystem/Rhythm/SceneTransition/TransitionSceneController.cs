using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.SceneTransition
{
    public class TransitionSceneController : MonoBehaviour
    {
        [SerializeField] private SceneTransitionSenderSO _sender;
        [SerializeField] private GameObject _landscapeGuideObj; 
        [SerializeField] private GameObject _portraitGuideObj;  

        private async void Start()
        {
            if (_sender == null)
            {
                Debug.LogError("TransitionSceneController: SceneTransitionSenderSO is missing!");
                return;
            }

            string nextScene = _sender.NextSceneName;
            bool toLandscape = _sender.Mode == TransitionMode.ToLandscape;

            if (_landscapeGuideObj) _landscapeGuideObj.SetActive(toLandscape);
            if (_portraitGuideObj) _portraitGuideObj.SetActive(!toLandscape);

            if (toLandscape)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                await UniTask.WaitUntil(() => Screen.width > Screen.height); 
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
                await UniTask.WaitUntil(() => Screen.height > Screen.width); 
            }

            await UniTask.Delay(1000);

            SceneManager.LoadScene(nextScene);
        }
    }
}