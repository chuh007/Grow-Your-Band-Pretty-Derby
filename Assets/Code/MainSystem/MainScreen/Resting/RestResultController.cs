using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Resting
{
    public class RestResultController : MonoBehaviour
    {
        [SerializeField] private Transform sdRoot;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private StatManager statManager;
        
        public async UniTask PlayRestSequence(
            UnitDataSO unit,
            float beforeHealth,
            float afterHealth)
        {
            var idleSDPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Rest/SD/Idle");
            var idleInstance = Instantiate(idleSDPrefab, sdRoot);

            var bar = idleInstance.GetComponentInChildren<TrainingProgressBar>();
            if (bar != null)
                await bar.Play(1f);

            Destroy(idleInstance);

            var resultPrefab =
                await GameManager.Instance.LoadAddressableAsync<GameObject>("Rest/UI/Result");
            var resultInstance = Instantiate(resultPrefab, uiRoot);
            var resultUI = resultInstance.GetComponent<RestResultUI>();

            await resultUI.Play(
                idleSprite: null,
                resultSprite: null,
                () => gameObject.SetActive(false),
                beforeHealth: beforeHealth,
                maxHealth: unit.maxCondition
            );

            await UniTask.Delay(300);

            resultUI.SetHealth(afterHealth, unit.maxCondition);
        }
    }
}