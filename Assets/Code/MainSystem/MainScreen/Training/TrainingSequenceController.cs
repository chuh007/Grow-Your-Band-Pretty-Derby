using System.Collections.Generic;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class TrainingSequenceController : MonoBehaviour
    {
        [SerializeField] private Transform sdRoot;
        [SerializeField] private Transform uiRoot;

        private GameObject _currentSD;

        public async UniTask PlayTrainingSequence(bool isSuccsed, PersonalpracticeDataSO personalPracticeDataSO)
        {
            var idleSDPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/SD/Idle");
            var idleSDImage = await GameManager.Instance.LoadAddressableAsync<Sprite>(personalPracticeDataSO.ProgressImageAddresableKey);
            var idleSDInstance = Instantiate(idleSDPrefab, sdRoot);
            var progressImageUI = idleSDInstance.GetComponent<TrainingProgressImage>();
            if (progressImageUI != null)
                progressImageUI.SetProgressImage(idleSDImage);

            var progressBar = idleSDInstance.GetComponentInChildren<TrainingProgressBar>();
            if (progressBar != null)
                await progressBar.Play(1.0f);

            Destroy(idleSDInstance);

            
            var resultUIPrefab = await GameManager.Instance.LoadAddressableAsync<GameObject>("Training/UI/Result");
            var resultUIInstance = Instantiate(resultUIPrefab, uiRoot);
            var resultUI = resultUIInstance.GetComponent<TrainingResultUI>();
            
            var idleSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>("Training/Sprite/Idle");
            var resultSprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(
                isSuccsed ? personalPracticeDataSO.SuccseImageAddressableKey : personalPracticeDataSO.FaillImageAddressableKey);
            
            var statList = new List<(Sprite, int, int)>
            {
                (await GameManager.Instance.LoadAddressableAsync<Sprite>("Icon/Performance"), 100, isSuccsed ? 30 : 0),
                (await GameManager.Instance.LoadAddressableAsync<Sprite>("Icon/Sense"), 80, isSuccsed ? 20 : 0),
            };
            
            await resultUI.Play(idleSprite, resultSprite, statList, isSuccsed, () =>
            {
                Debug.Log("결과창 닫힘");
            });
        }


        private void Cleanup()
        {
            foreach (Transform child in sdRoot)
                Destroy(child.gameObject);
            foreach (Transform child in uiRoot)
                Destroy(child.gameObject);
        }
    }
}