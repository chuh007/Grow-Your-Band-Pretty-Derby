using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.MainSystem.Rhythm.SceneTransition
{
    public class TransitionSceneController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private SceneTransitionSenderSO _sender;

        [Header("UI Objects")]
        [SerializeField] private GameObject _landscapeGuideObj; // 가로 -> 세로
        [SerializeField] private GameObject _portraitGuideObj; // 세로 -> 가로

        [Header("Settings")]
        [SerializeField] private float _minDuration = 1.0f; // 최소 대기 시간

        private async void Start()
        {
            if (_sender == null)
            {
                Debug.LogError("TransitionSceneController: SceneTransitionSenderSO is missing!");
                return;
            }

            string nextScene = _sender.NextSceneName;
            TransitionMode mode = _sender.Mode;

            if (mode == TransitionMode.ToLandscape)
            {
                if (_landscapeGuideObj) _landscapeGuideObj.SetActive(true);
                if (_portraitGuideObj) _portraitGuideObj.SetActive(false);
                
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                
                await UniTask.WaitUntil(() => Screen.width > Screen.height);
            }
            else
            {
                if (_landscapeGuideObj) _landscapeGuideObj.SetActive(false);
                if (_portraitGuideObj) _portraitGuideObj.SetActive(true);
                
                Screen.orientation = ScreenOrientation.Portrait;

                await UniTask.WaitUntil(() => Screen.height > Screen.width);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_minDuration));

            SceneManager.LoadScene(nextScene);
        }
    }
}
