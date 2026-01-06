using System.Collections.Generic;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training.Code.MainSystem.MainScreen.Training;
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
        
        private GameObject personalIdleInstance;
        private TrainingProgressBar bar;
        private TrainingProgressImage progressImage;

        private GameObject personalResultInstance;
        private TrainingResultUI personalResultUI;
        
        private GameObject teamIdleInstance;
        private TeamIdleController teamIdleController;

        private GameObject teamResultInstance;
        private TeamTrainingResultUI teamResultUI;
        
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

            await LoadPersonalIdle(trainingType.GetIdlePrefabKey());
            await PlayPersonalStart(trainingType);

            var idleSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetIdleImageKey());
            var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                trainingType.GetResultImageKey(isSuccess));

            var statList = personalProvider.GetStatChanges(unit, statManager, isSuccess);

            await LoadPersonalResult(trainingType.GetResultUIPrefabKey());

            await personalResultUI.Play(
                idleSprite,
                resultSprite,
                statList,
                isSuccess,
                unit.currentCondition,
                () =>
                {
                    personalResultInstance.SetActive(false);
                    gameObject.SetActive(false);
                }
            );
        }
        
        public async UniTask PlayTeamTrainingSequence(bool isSuccess, List<UnitDataSO> units)
        {
            await LoadTeamIdle("Training/SD/TeamIdle", units);

            var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                isSuccess ? "Training/Sprite/TeamSuccess" : "Training/Sprite/TeamFail");

            var allStats = new Dictionary<UnitDataSO, List<(string name, Sprite icon, int baseValue, int delta)>>();

            foreach (var unit in units)
            {
                var statList = new List<(string, Sprite, int, int)>();
                foreach (var stat in unit.stats)
                {
                    var memberStat = statManager.GetMemberStat(unit.memberType, stat.statType);
                    int delta = isSuccess ? 1 : 0;

                    statList.Add((
                        stat.statName,
                        stat.statIcon,
                        Mathf.RoundToInt(memberStat.CurrentValue),
                        delta
                    ));
                }

                allStats[unit] = statList;
            }

            await LoadTeamResult("Training/UI/TeamResult");

            await teamResultUI.PlayTeamResult(
                resultSprite,
                allStats,
                isSuccess,
                () =>
                {
                    teamResultInstance.SetActive(false);
                    gameObject.SetActive(false);
                });
        }
        
        private async UniTask LoadPersonalIdle(string prefabPath)
        {
            if (personalIdleInstance == null)
            {
                var idlePrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabPath);
                personalIdleInstance = Instantiate(idlePrefab, sdRoot);
                bar = personalIdleInstance.GetComponentInChildren<TrainingProgressBar>();
                progressImage = personalIdleInstance.GetComponent<TrainingProgressImage>();
            }

            personalIdleInstance.SetActive(true);
            bar?.ResetBar();
        }
        
        private async UniTask LoadPersonalResult(string prefabPath)
        {
            if (personalResultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabPath);
                personalResultInstance = Instantiate(prefab, uiRoot);
                personalResultUI = personalResultInstance.GetComponent<TrainingResultUI>();
            }

            personalResultInstance.SetActive(true);
        }
        
        private async UniTask LoadTeamIdle(string prefabPath, List<UnitDataSO> units)
        {
            if (teamIdleInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabPath);
                teamIdleInstance = Instantiate(prefab, sdRoot);
                teamIdleController = teamIdleInstance.GetComponent<TeamIdleController>();
            }

            teamIdleInstance.SetActive(true);

            if (teamIdleController != null)
            {
                teamIdleController.Setup(units);
                await teamIdleController.PlayAllBars();
                await UniTask.Delay(800);
                teamIdleController.HideAll();
            }
        }
        private async UniTask LoadTeamResult(string prefabPath)
        {
            if (teamResultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabPath);
                teamResultInstance = Instantiate(prefab, uiRoot);
                teamResultUI = teamResultInstance.GetComponent<TeamTrainingResultUI>();
            }

            teamResultInstance.SetActive(true);
        }
        
        private async UniTask PlayPersonalStart(ITrainingType trainingType)
        {
            var progressSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetProgressImageKey());

            progressImage?.SetProgressImage(progressSprite);

            if (bar != null)
                await bar.Play(1f);

            await UniTask.Delay(800);
            personalIdleInstance.SetActive(false);
        }
    }
}
