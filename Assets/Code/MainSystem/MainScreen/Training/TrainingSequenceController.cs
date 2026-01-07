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
    public class TrainingSequenceController : MonoBehaviour
    {
        [SerializeField] private Transform sdRoot;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private StatManager statManager;

        private GameObject idleInstance;
        private TrainingProgressBar bar;
        private TrainingProgressImage progressImage;
        private TeamTrainingProgressImage teamProgressImage;

        private GameObject resultInstance;
        private TrainingResultUI resultUI;
        public async UniTask PlayTrainingSequence(
            bool isSuccess,
            ITrainingType trainingType,
            UnitDataSO unit)
        {
            if (trainingType is not IStatChangeProvider personalProvider)
            {
                Debug.LogError("TrainingType이 개인 훈련 타입이 아닙니다.");
                return;
            }

            await SetupBar(trainingType);
            await ShowPersonalProgress(trainingType);

            var idleSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetIdleImageKey());
            var resultSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetResultImageKey(isSuccess));

            var statList = personalProvider.GetStatChanges(unit, statManager, isSuccess);

            await EnsureResultUI();

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
                }
            );
        }
        
        public async UniTask PlayTeamTrainingSequence(
            bool isSuccess,
            ITrainingType trainingType,
            List<UnitDataSO> units)
        {
            if (trainingType is not ITeamStatChangeProvider teamProvider)
            {
                Debug.LogError("TrainingType이 팀 훈련 타입이 아닙니다.");
                return;
            }

            await SetupBar(trainingType);
            await ShowTeamProgress(trainingType, units);

            var idleSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetIdleImageKey());
            var resultSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetResultImageKey(isSuccess));

            var allStats = teamProvider.GetAllStatChanges(units, statManager, isSuccess);

            await EnsureResultUI();

            await resultUI.PlayTeamResult(
                idleSprite,
                resultSprite,
                allStats,
                isSuccess,
                () =>
                {
                    resultInstance.SetActive(false);
                    gameObject.SetActive(false);

                }
            );
        }

        private async UniTask SetupBar(ITrainingType trainingType)
        {
            if (idleInstance == null)
            {
                string prefabKey = trainingType.GetIdlePrefabKey(); 
                var idleSDPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabKey);
                idleInstance = Instantiate(idleSDPrefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                progressImage = idleInstance.GetComponent<TrainingProgressImage>();
                teamProgressImage = idleInstance.GetComponent<TeamTrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar?.ResetBar();
        }

        
        private async UniTask ShowPersonalProgress(ITrainingType trainingType)
        {
            var progressSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetProgressImageKey());
            progressImage?.SetProgressImage(progressSprite);

            if (bar != null)
                await bar.Play(1f);

            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }
        
        private async UniTask ShowTeamProgress(ITrainingType trainingType, List<UnitDataSO> selectedUnits)
        {
            var progressSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetProgressImageKey());

            var selectedTypes = new HashSet<MemberType>();
            foreach (var unit in selectedUnits)
                selectedTypes.Add(unit.memberType);

            teamProgressImage?.SetProgressImages(progressSprite, selectedTypes);

            if (bar != null)
                await bar.Play(1f);

            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }



        private async UniTask EnsureResultUI()
        {
            if (resultInstance == null)
            {
                var resultPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/UI/Result");
                resultInstance = Instantiate(resultPrefab, uiRoot);
                resultUI = resultInstance.GetComponent<TrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }
    }
}
