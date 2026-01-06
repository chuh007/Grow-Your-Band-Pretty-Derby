using System.Collections.Generic;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
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

        private GameObject resultInstance;
        private TrainingResultUI resultUI;

        public async UniTask PlayTrainingSequence(
            bool isSuccess,
            PersonalpracticeDataSO practiceData,
            UnitDataSO unit)
        {
            if (idleInstance == null)
            {
                var idleSDPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/SD/Idle");
                idleInstance = Instantiate(idleSDPrefab, sdRoot);
                bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
                progressImage = idleInstance.GetComponent<TrainingProgressImage>();
            }

            idleInstance.SetActive(true);
            bar?.ResetBar();

            var progressSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(practiceData.ProgressImageAddresableKey);
            progressImage?.SetProgressImage(progressSprite);
            
            if (bar != null)
                await bar.Play(1f);

            await UniTask.Delay(800);
            idleInstance.SetActive(false);
            
            if (resultInstance == null)
            {
                var resultPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/UI/Result");
                resultInstance = Instantiate(resultPrefab, uiRoot);
                resultUI = resultInstance.GetComponent<TrainingResultUI>();
            }

            resultInstance.SetActive(true);

            var idleSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(practiceData.IdleImageAddressableKey);
            var resultSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(
                    isSuccess
                        ? practiceData.SuccseImageAddressableKey
                        : practiceData.FaillImageAddressableKey);

            StatType targetType = practiceData.PracticeStatType;

            var statList = new List<(string name, Sprite icon, int baseValue, int delta)>();
            for (int i = 0; i < unit.stats.Count && statList.Count < 4; i++)
            {
                var stat = unit.stats[i];
                var memberStat = statManager.GetMemberStat(unit.memberType, stat.statType);

                int delta = (isSuccess && stat.statType == targetType)
                    ? Mathf.RoundToInt(practiceData.statIncrease)
                    : 0;

                statList.Add((
                    stat.statName,
                    stat.statIcon,
                    Mathf.RoundToInt(memberStat.CurrentValue),
                    delta
                ));
            }

            await resultUI.Play(
                idleSprite,
                resultSprite,
                statList,
                isSuccess,
                () =>
                {
                    resultInstance.SetActive(false);
                    gameObject.SetActive(false);
                }
            );
        }
    }
}
