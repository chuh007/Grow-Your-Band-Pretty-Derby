using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    
    /// <summary>
    /// 개인 훈련 연습 연출
    /// </summary>
    
    public class PersonalTrainingSequenceController : MonoBehaviour
    {
        [SerializeField] private Transform sdRoot;
        [SerializeField] private Transform uiRoot;

        private GameObject idleInstance;
        private TrainingProgressBar bar;
        private TrainingProgressImage progressImage;

        private GameObject resultInstance;
        private TrainingResultUI resultUI;

        public async UniTask PlayTrainingSequence(
            bool isSuccess,
            ITrainingType trainingType,
            UnitDataSO unit)
        {
            if (trainingType is not IStatChangeProvider personalProvider)
            {
                Debug.LogError("개인 훈련 타입 아님");
                return;
            }

            await SetupBar(trainingType);
            await ShowPersonalProgress(trainingType);

            var idleSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetIdleImageKey());
            var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetResultImageKey(isSuccess));
            var statList = personalProvider.GetStatChanges(unit, StatManager.Instance, isSuccess);

            await ShowResultUI();

            await resultUI.Play(
                idleSprite,
                resultSprite,
                statList,
                isSuccess,
                unit.currentCondition,
                () =>
                {
                    resultInstance.SetActive(false);
                    gameObject.SetActive(false);
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                });
        }

        private async UniTask SetupBar(ITrainingType trainingType)
        {
            if (idleInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(trainingType.GetIdlePrefabKey());
                idleInstance = Instantiate(prefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                progressImage = idleInstance.GetComponent<TrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar.ResetBar();
        }

        private async UniTask ShowPersonalProgress(ITrainingType trainingType)
        {
            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetProgressImageKey());
            progressImage.SetProgressImage(sprite);

            await bar.Play(1f);
            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }

        private async UniTask ShowResultUI()
        {
            if (resultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/UI/Result");
                resultInstance = Instantiate(prefab, uiRoot);
                resultUI = resultInstance.GetComponent<TrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }
    }
}
