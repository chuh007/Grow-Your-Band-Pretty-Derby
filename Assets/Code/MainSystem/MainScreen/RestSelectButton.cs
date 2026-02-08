using System;
using System.Collections.Generic;
using Code.Core.Addressable;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.CutsceneEvents;
using Code.MainSystem.Dialogue;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Resting;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class RestSelectButton : MonoBehaviour
    {
        [SerializeField] private List<AssetReferenceT<RestDialogueInfoSO>> restDialogues;
        [SerializeField] private MainScreen mainScreen;
        
        private readonly Dictionary<MemberType, DialogueInformationSO> _restDialogues = new Dictionary<MemberType, DialogueInformationSO>();
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(UnitSelect);
            
            InitRestDialogues();
        }

        private async void InitRestDialogues()
        {
            try
            {
                foreach (var assetRef in restDialogues)
                {
                    RestDialogueInfoSO info = await GameResourceManager.Instance.LoadAsync<RestDialogueInfoSO>(assetRef.RuntimeKey.ToString());

                    if (info == null || info.RestInformation == null)
                        continue;
                
                    DialogueInformationSO dialogueData = await GameResourceManager.Instance.LoadAsync<DialogueInformationSO>(info.RestInformation.RuntimeKey.ToString());
                    
                    if (dialogueData != null)
                        _restDialogues.TryAdd(info.MemberType, dialogueData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[RestSelectButton] 리소스 로드 중 예외 발생: {e.Message}\n{e.StackTrace}");
            }
        }

        private void UnitSelect()
        {
            UnitDataSO currentUnit = mainScreen.UnitSelector.CurrentUnit;
            if (currentUnit == null) 
                return;

            if (TrainingManager.Instance.IsMemberTrained(currentUnit.memberType))
                return;
            
            if (_restDialogues.TryGetValue(currentUnit.memberType, out var dialogueSO))
                Bus<DialogCutscenePlayEvent>.Raise(new DialogCutscenePlayEvent(dialogueSO));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}