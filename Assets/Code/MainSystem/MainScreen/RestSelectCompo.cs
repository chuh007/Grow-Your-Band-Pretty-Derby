using System;
using System.Collections.Generic;
using Code.Core.Addressable;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.CutsceneEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Dialogue;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Resting;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class RestSelectCompo : MonoBehaviour
    {
        [SerializeField] private List<AssetReferenceT<RestDialogueInfoSO>> restDialogues;
        [SerializeField] private MainScreen mainScreen;
        [SerializeField] private HealthBar healthBar;
        
        [Header("Button Settings")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color confirmColor = Color.gray;
        
        private readonly Dictionary<MemberType, DialogueInformationSO> _restDialogues = new Dictionary<MemberType, DialogueInformationSO>();
        
        private UnitDataSO _lastSelectedUnit; 
        private const float HealAmount = 10f; // 임시 고정 회복량
        private bool _isWaitingForDialogue = false;

        private void Awake()
        {
            button.onClick.AddListener(UnitSelect);
            InitRestDialogues();
        }
        
        private void OnEnable()
        {
            // 대화 종료 이벤트 구독
            Bus<DialogueEndEvent>.OnEvent += HandleDialogueEnd;
        }

        private void OnDisable()
        {
            // 이벤트 해제
            Bus<DialogueEndEvent>.OnEvent -= HandleDialogueEnd;
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
            if (currentUnit == null || currentUnit.currentCondition >= currentUnit.maxCondition)
                return;
            
            if (TrainingManager.Instance.IsMemberTrained(currentUnit.memberType))
            {
                _lastSelectedUnit = null;
                return;
            }
            
            if (_lastSelectedUnit != currentUnit)
            {
                _lastSelectedUnit = currentUnit;
                ShowHealPreview(currentUnit);
                
                buttonText.color = confirmColor;
                return;
            }
            
            if (!_restDialogues.TryGetValue(currentUnit.memberType, out var dialogueSO))
                return;

            _isWaitingForDialogue = true;
            Bus<DialogCutscenePlayEvent>.Raise(new DialogCutscenePlayEvent(dialogueSO));
            ProcessConfirmRest(currentUnit);
            
            ResetButtonState();
        }
        
        private void ResetButtonState()
        {
            _lastSelectedUnit = null;
            buttonText.color = normalColor;
        }
        
        private void HandleDialogueEnd(DialogueEndEvent evt)
        {
            if (!_isWaitingForDialogue) 
                return;
            
            _isWaitingForDialogue = false;
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
        }
        
        /// <summary>
        /// 첫 번째 클릭 시: 체력 바에 회복될 양을 미리 보여줌
        /// </summary>
        private void ShowHealPreview(UnitDataSO unit)
        {
            var holder = TraitManager.Instance.GetHolder(unit.memberType);
            float rewardValue = holder.GetCalculatedStat(TraitTarget.PracticeCondition, HealAmount);
            healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            healthBar.PrevieMinusHealth(-rewardValue);
        }

        /// <summary>
        /// 두 번째 클릭 시: 실제 데이터 적용 및 UI 최종 갱신
        /// </summary>
        private void ProcessConfirmRest(UnitDataSO selectedUnit)
        {
            var holder = TraitManager.Instance.GetHolder(selectedUnit.memberType);
            float rewardValue = holder.GetCalculatedStat(TraitTarget.PracticeCondition, HealAmount);
            float beforeHealth = selectedUnit.currentCondition;
            float afterHealth = Mathf.Min(beforeHealth + rewardValue, selectedUnit.maxCondition);
            
            selectedUnit.currentCondition = afterHealth;
            
            TrainingManager.Instance.MarkMemberTrained(selectedUnit.memberType);
            Bus<ConfirmRestEvent>.Raise(new ConfirmRestEvent(selectedUnit));
            
            healthBar.SetHealth(afterHealth, selectedUnit.maxCondition);
            healthBar.PrevieMinusHealth(-rewardValue); 
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}