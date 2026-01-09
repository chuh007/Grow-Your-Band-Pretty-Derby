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
                }
            );
        }

        public async UniTask PlayTeamTrainingSequence(
            bool isSuccess,
            ITeamTraingType trainingType,
            List<UnitDataSO> units)
        {
            if (trainingType is not ITeamStatChangeProvider teamProvider)
            {
                Debug.LogError("TrainingType이 팀 훈련 타입이 아닙니다.");
                return;
            }

            var unitSnapshot = new List<UnitDataSO>(units);
            var allStats = teamProvider.GetAllStatChanges(unitSnapshot, statManager, isSuccess);

            await SetupBar(trainingType, memberType: unitSnapshot[0].memberType); 
            await ShowTeamProgress(trainingType, new List<MemberType>(unitSnapshot.ConvertAll(u => u.memberType)));

            var memberResultSprites = new Dictionary<MemberType, Sprite>();
            foreach (var unit in unitSnapshot)
            {
                string resultKey = trainingType.GetResultImageKey(isSuccess, unit.memberType);
                var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(resultKey);
                memberResultSprites[unit.memberType] = resultSprite;
            }

            await TeamTraingResultUI();

            await teamresultUI.PlayTeamResult(
                resultSprite: null, 
                memberResultSprites,
                () =>
                {
                    resultInstance.SetActive(false);
                    gameObject.SetActive(false);
                    bottomTab.ExitModeEvent.Invoke(2);
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
            }

            idleInstance.SetActive(true);
            bar?.ResetBar();
        }
        
        private async UniTask SetupBar(ITeamTraingType trainingType, MemberType memberType)
        {
            if (idleInstance == null)
            {
                string prefabKey = trainingType.GetIdlePrefabKey();
                var idleSDPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(prefabKey);
                idleInstance = Instantiate(idleSDPrefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
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
        
        private async UniTask ShowTeamProgress(ITeamTraingType trainingType, List<MemberType> memberTypes)
        {
            var activeMembers = new HashSet<MemberType>();
            Dictionary<MemberType, Sprite> memberSprites = new();

            foreach (var memberType in memberTypes)
            {
                string progressKey = trainingType.GetProgressImageKey(memberType);
                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(progressKey);
                memberSprites[memberType] = sprite;
                activeMembers.Add(memberType);
            }

            foreach (var kvp in memberSprites)
            {
                teamProgressImage?.SetProgressImages(kvp.Value, activeMembers);
            }

            if (bar != null)
                await bar.Play(1f);

            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }

        
        private async UniTask TeamTraingResultUI()
        {
            if (resultInstance == null)
            {
                var resultPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Concert/UI/Result");
                resultInstance = Instantiate(resultPrefab, uiRoot);
                teamresultUI = resultInstance.GetComponent<TeamTrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }



        private async UniTask TraingResultUI()
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
