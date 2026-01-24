using System;
using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.MainScreen.Training
{
    public class PersonalPracticeSequencePlayer : MonoBehaviour
    {
        [SerializeField] private Transform spawnRoot;
        [SerializeField] private PracticeResultWindow practiceResultWindow;

        private GameObject _instance;
        private PersonalPracticeView _view;
        [Inject] private StatManager statManager;

        private MemberActionData _actionData;

        public async UniTask Play(
            UnitDataSO unitData,
            bool isSuccess,
            PersonalpracticeDataSO dataSo,
            float currentCondition,
            StatData teamStatData,
            float teamStatDelta)
        {
            if (_instance == null)
            {
                var prefab = await Addressables.LoadAssetAsync<GameObject>("Training/Production/PersonalPractice").Task;
                _instance = Instantiate(prefab, spawnRoot);
                _view = _instance.GetComponent<PersonalPracticeView>();
            }

            foreach (var unitAction in unitData.unitActions)
            {
                if (unitAction.statType == dataSo.PracticeStatType)
                {
                    _actionData = unitAction;
                    break;
                }
            }

            _instance.SetActive(true);
            var tcs = new UniTaskCompletionSource();

            int receivedDelta = 0;

            await _view.Play(
                _actionData,
                isSuccess,
                dataSo,
                currentCondition,
                statManager,
                unitData.name,
                (statDelta) =>
                {
                    receivedDelta = statDelta;
                    tcs.TrySetResult(); 
                });

            await tcs.Task; 
            _view.gameObject.SetActive(false);
            await practiceResultWindow.Play(
                statManager,
                new List<UnitDataSO> { unitData },
                currentCondition,
                isSuccess ? dataSo.statIncrease : -dataSo.StaminaReduction,
                teamStatData,
                teamStatDelta,
                new Dictionary<(MemberType, StatType), int>
                {
                    { (unitData.memberType, dataSo.PracticeStatType), receivedDelta }
                },
                isSuccess,
                dataSo 
            );


            _instance.SetActive(false);
        }

    }
}
