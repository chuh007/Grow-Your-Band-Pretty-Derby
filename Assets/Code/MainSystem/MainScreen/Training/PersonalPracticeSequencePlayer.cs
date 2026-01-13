using System;
using Code.Core;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Code.MainSystem.MainScreen.Training
{
    public class PersonalPracticeSequencePlayer : MonoBehaviour
    {
        [SerializeField] private Transform spawnRoot;
        private GameObject _instance;
        private PersonalPracticeView _view;
        private MemberActionData _actionData;
        [Inject] private StatManager statManager;

        public async UniTask Play(UnitDataSO unitData, bool isSuccess,PersonalpracticeDataSO dataSo)
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
                }
            }

            _instance.SetActive(true);

            await _view.Play(_actionData, isSuccess, dataSo,unitData.currentCondition,statManager);

            _instance.SetActive(false);
        }
        
        

    }
}