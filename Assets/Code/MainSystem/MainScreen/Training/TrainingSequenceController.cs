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

        public async UniTask PlayTrainingSequence(
            bool isSuccess,
            PersonalpracticeDataSO practiceData,
            UnitDataSO unit)
        {
            var idleSDPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/SD/Idle");
            var progressSprite =
                await GameManager.Instance.LoadAddressableAsync<Sprite>(practiceData.ProgressImageAddresableKey);

            var idleInstance = Instantiate(idleSDPrefab, sdRoot);
            idleInstance.GetComponent<TrainingProgressImage>()
                ?.SetProgressImage(progressSprite);

            var bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
            if (bar != null)
                await bar.Play(1f);

            Destroy(idleInstance);
            
            var resultPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/UI/Result");
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<TrainingResultUI>();

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

                int delta =
                    (isSuccess && stat.statType == targetType)
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
                () => gameObject.SetActive(false)
            );

        }
    }
}
