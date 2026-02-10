using System;
using System.Threading;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Code.MainSystem.Rhythm.Data;

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

            string nextScene = _sender.nextSceneName;
            bool toLandscape = _sender.mode == TransitionMode.ToLandscape;

            if (_landscapeGuideObj) _landscapeGuideObj.SetActive(toLandscape);
            if (_portraitGuideObj) _portraitGuideObj.SetActive(!toLandscape);

            // 타임아웃 설정을 위한 CTS 생성
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(RhythmGameBalanceConsts.TRANSITION_TIMEOUT_MS);

            if (toLandscape)
            {
                Bus<ScreenRotationEvent>.Raise(new ScreenRotationEvent(false));
            }
            else
            {
                Bus<ScreenRotationEvent>.Raise(new ScreenRotationEvent(true));
            }
            
           

            // try 
            // {
            //     if (toLandscape)
            //     {
            //         Screen.orientation = ScreenOrientation.LandscapeLeft;
            //         await UniTask.WaitUntil(() => Screen.width > Screen.height, cancellationToken: cts.Token); 
            //     }
            //     else
            //     {
            //         Screen.orientation = ScreenOrientation.Portrait;
            //         await UniTask.WaitUntil(() => Screen.height > Screen.width, cancellationToken: cts.Token); 
            //     }
            // }
            // catch (OperationCanceledException)
            // {
            //     Debug.LogWarning($"TransitionSceneController: Orientation change timed out after {RhythmGameBalanceConsts.TRANSITION_TIMEOUT_MS}ms. Proceeding anyway.");
            // }

            await UniTask.Delay(RhythmGameBalanceConsts.MIN_LOADING_TIME_MS);

            SceneManager.LoadScene(nextScene);
        }
    }
}