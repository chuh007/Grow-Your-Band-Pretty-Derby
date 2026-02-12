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
                if (toLandscape)
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                    await WaitForStableOrientation(true, cts.Token);
                }
                else
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                    await WaitForStableOrientation(false, cts.Token);
                }
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

        /// <summary>
        /// 해상도와 Safe Area가 완전히 안정화될 때까지 대기합니다.
        /// </summary>
        /// <param name="toLandscape">가로 모드로 전환 여부 (true: 가로, false: 세로)</param>
        /// <param name="token">비동기 작업 취소를 위한 토큰</param>
        private async UniTask WaitForStableOrientation(bool toLandscape, CancellationToken token)
        {
            // 0. 초기 안정화 대기: 회전 명령 직후 OS 처리를 위한 유예 (2프레임)
            await UniTask.DelayFrame(2, PlayerLoopTiming.Update, token);

            // 1. 1차 대기: 단순히 너비/높이 역전이 일어날 때까지 대기
            await UniTask.WaitUntil(() => toLandscape ? Screen.width > Screen.height : Screen.height > Screen.width, cancellationToken: token);

            // 2. 2차 대기: 수치가 '완전히' 고정될 때까지 검사 (15프레임 연속 유지 조건)
            int stableFrames = 0;
            const int RequiredStableFrames = 15;
            int lastWidth = Screen.width;
            int lastHeight = Screen.height;
            Rect lastSafeArea = Screen.safeArea;

            while (stableFrames < RequiredStableFrames) 
            {
                token.ThrowIfCancellationRequested();
                
                // 프레임의 가장 마지막 시점(렌더링 직전/직후)에 체크하여 중간 값을 피함
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, token); 

                bool resolutionStable = (Screen.width == lastWidth && Screen.height == lastHeight);
                bool safeAreaStable = (Screen.safeArea == lastSafeArea);
                bool orientationCorrect = toLandscape ? Screen.width > Screen.height : Screen.height > Screen.width;

                if (resolutionStable && safeAreaStable && orientationCorrect)
                {
                    stableFrames++;
                }
                else
                {
                    // 수치가 튀면 카운트 리셋하고 다시 기준값 잡음
                    stableFrames = 0;
                    lastWidth = Screen.width;
                    lastHeight = Screen.height;
                    lastSafeArea = Screen.safeArea;
                }
            }
            
            // 3. 엔진 내부 캔버스 갱신 강제 수행 (남아있는 더티 플래그 해소)
            Canvas.ForceUpdateCanvases();

            // 4. 강제 업데이트가 렌더링에 반영되도록 한 프레임 대기
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }
}
