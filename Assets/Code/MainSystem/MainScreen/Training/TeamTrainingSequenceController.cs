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
    public class TeamTrainingSequenceController : MonoBehaviour
    {
        [SerializeField] private Transform sdRoot;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private StatManager statManager;
        [SerializeField] private BottomTab bottomTab;

        private GameObject idleInstance;
        private TrainingProgressBar bar;
        private TeamTrainingProgressImage teamProgressImage;

        private GameObject resultInstance;
        private TeamTrainingResultUI teamresultUI;

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
            await ShowTeamProgress(trainingType, unitSnapshot.ConvertAll(u => u.memberType));

            var memberResultSprites = new Dictionary<MemberType, Sprite>();
            foreach (var unit in unitSnapshot)
            {
                var resultKey = trainingType.GetResultImageKey(isSuccess, unit.memberType);
                memberResultSprites[unit.memberType] = await GameManager.Instance.LoadAddressableAsync<Sprite>(resultKey);
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

            await ShowResultUI();

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
                    Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
                });
        }

        private async UniTask SetupBar(ITeamTraingType trainingType, MemberType memberType)
        {
            if (idleInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>(trainingType.GetIdlePrefabKey());
                idleInstance = Instantiate(prefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                teamProgressImage = idleInstance.GetComponent<TeamTrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar.ResetBar();
        }

        private async UniTask ShowTeamProgress(ITeamTraingType trainingType, List<MemberType> memberTypes)
        {
            var activeMembers = new HashSet<MemberType>();

            foreach (var memberType in memberTypes)
            {
                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(trainingType.GetProgressImageKey(memberType));
                activeMembers.Add(memberType);
                teamProgressImage.SetProgressImages(sprite, activeMembers);
            }

            await bar.Play(1f);
            await UniTask.Delay(800);
            idleInstance.SetActive(false);
        }

        private async UniTask ShowResultUI()
        {
            if (resultInstance == null)
            {
                var prefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Concert/UI/Result");
                resultInstance = Instantiate(prefab, uiRoot);
                teamresultUI = resultInstance.GetComponent<TeamTrainingResultUI>();
            }

            resultInstance.SetActive(true);
        }
    }
}
