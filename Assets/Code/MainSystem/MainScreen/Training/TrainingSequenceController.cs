using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.MainScreen.Bottom;
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
        [SerializeField] private BottomTab bottomTab;

        private GameObject idleInstance;
        private TrainingProgressBar bar;
        private TrainingProgressImage progressImage;
        private TeamTrainingProgressImage teamProgressImage;

        private GameObject resultInstance;
        private TrainingResultUI resultUI;
        private TeamTrainingResultUI teamresultUI;

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

            var idleSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                trainingType.GetIdleImageKey());

            var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                trainingType.GetResultImageKey(isSuccess));

            var statList = personalProvider.GetStatChanges(unit, statManager, isSuccess);

            await TraingResultUI();

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

        public async UniTask PlayTeamTrainingSequence(
            bool isSuccess,
            ITeamTraingType trainingType,
            List<UnitDataSO> units)
        {
            if (trainingType is not ITeamStatChangeProvider teamProvider)
            {
                Debug.LogError("팀 훈련 타입 아님");
                return;
            }

            var unitSnapshot = new List<UnitDataSO>(units);
            var allStats = teamProvider.GetAllStatChanges(unitSnapshot, statManager, isSuccess);

            await SetupBar(trainingType, unitSnapshot[0].memberType);
            await ShowTeamProgress(
                trainingType,
                new List<MemberType>(unitSnapshot.ConvertAll(u => u.memberType)));

            var memberResultSprites = new Dictionary<MemberType, Sprite>();
            foreach (var unit in unitSnapshot)
            {
                var resultKey = trainingType.GetResultImageKey(isSuccess, unit.memberType);
                memberResultSprites[unit.memberType] =
                    await GameManager.Instance.LoadAddressableAsync<Sprite>(resultKey);
            }

            (string name, Sprite icon, int baseValue, int delta) teamStat = default;
            foreach (var stats in allStats.Values)
            {
                var harmony = stats.Find(s => s.name == "하모니");
                if (!string.IsNullOrEmpty(harmony.name))
                {
                    teamStat = harmony;
                    break;
                }
            }

            await TeamTraingResultUI();

            await teamresultUI.PlayTeamResult(
                null,
                memberResultSprites,
                teamStat,
                isSuccess,
                () =>
                {
                    resultInstance.SetActive(false);
                    gameObject.SetActive(false);
                    bottomTab.ExitModeEvent.Invoke(2);
                });
        }

        private async UniTask SetupBar(ITrainingType trainingType)
        {
            if (idleInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(
                    trainingType.GetIdlePrefabKey());
                idleInstance = Instantiate(prefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                progressImage = idleInstance.GetComponent<TrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar.ResetBar();
        }

        private async UniTask SetupBar(ITeamTraingType trainingType, MemberType memberType)
        {
            if (idleInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(
                    trainingType.GetIdlePrefabKey());
                idleInstance = Instantiate(prefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                teamProgressImage = idleInstance.GetComponent<TeamTrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar.ResetBar();
        }

        private async UniTask ShowPersonalProgress(ITrainingType trainingType)
        {
            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                trainingType.GetProgressImageKey());
            progressImage.SetProgressImage(sprite);

            await bar.Play(1f);
            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }

        private async UniTask ShowTeamProgress(
            ITeamTraingType trainingType,
            List<MemberType> memberTypes)
        {
            var activeMembers = new HashSet<MemberType>();

            foreach (var memberType in memberTypes)
            {
                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                    trainingType.GetProgressImageKey(memberType));
                activeMembers.Add(memberType);
                teamProgressImage.SetProgressImages(sprite, activeMembers);
            }

            await bar.Play(1f);
            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }

        private async UniTask TeamTraingResultUI()
        {
            if (resultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(
                    "Concert/UI/Result");
                resultInstance = Instantiate(prefab, uiRoot);
                teamresultUI = resultInstance.GetComponent<TeamTrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }

        private async UniTask TraingResultUI()
        {
            if (resultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(
                    "Training/UI/Result");
                resultInstance = Instantiate(prefab, uiRoot);
                resultUI = resultInstance.GetComponent<TrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }
    }
}
