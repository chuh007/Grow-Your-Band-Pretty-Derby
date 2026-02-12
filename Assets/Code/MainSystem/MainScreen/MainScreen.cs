using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.Core.Addressable;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.Turn;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class MainScreen : MonoBehaviour, ITurnStartComponent
    {
        [Header("Addressables Keys/Labels")] [SerializeField]
        private string unitLabel = "Units";

        [Header("UI")] [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> statNameTexts;
        [SerializeField] private List<TextMeshProUGUI> statValueTexts;
        [SerializeField] private List<Image> statIcons;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private Image characterIcon;
        [SerializeField] private GameObject teamPanel;
        [SerializeField] private List<Button> practiceBtns;
        [SerializeField] private AddressableLoadUI loadingUI;

        [Header("Button References")] [SerializeField]
        private Transform practiceButtonsParent;

        [Header("Member Carousel")] [SerializeField]
        private MemberCarousel memberCarousel;

        [Header("Components")] [SerializeField]
        private PersonalPracticeCompo personalPracticeCompo;

        [SerializeField] private TeamPracticeCompo teamPracticeCompo;

        public UnitSelector UnitSelector { get; private set; }

        private StatUIUpdater _statUIUpdater;
        private List<UnitDataSO> _loadedUnits;
        private UnitDataSO _currentUnit;

        private Dictionary<MemberType, bool> _memberActionCounts = new();
        private bool _isbuskingAvailable = true;

        #region Unity LifeCycle

        private async void Start()
        {
            // 총 로딩 단계 수 설정
            loadingUI.ShowLoadingUI(6);
            
            Debug.Log("[MainScreen] Start called");
            try
            {
                loadingUI.UpdateProgress("Initializing...");
                InitializeActionCounts();
                
                loadingUI.UpdateProgress("Loading Units...");
                await LoadUnitsAsync();

                loadingUI.UpdateProgress("Setting up UI...");
                // ForceEnableAllButtons 삭제 - UpdateButton()이 알아서 처리함
                
                loadingUI.UpdateProgress("Refreshing Layout...");
                StartCoroutine(RefreshUILayout());
                
                loadingUI.UpdateProgress("Finalizing...");
                await Task.Delay(100);
                
                // 로딩 완료 후 버튼 상태 업데이트
                UpdateButtonsAfterLoad();
                
                loadingUI.UpdateProgress("Complete!");
            }
            catch (Exception e)
            {
                Debug.LogError($"[MainScreen] Error in Start: {e.Message}\n{e.StackTrace}");
                loadingUI.HideLoadingUI();
                throw;
            }
        }
        
        private IEnumerator RefreshUILayout()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<RectTransform>());
        }

        private void OnEnable()
        {
            Debug.Log("[MainScreen] OnEnable called");
            Bus<MemberTrainingStateChangedEvent>.OnEvent += OnMemberTrainingStateChanged;

            // ForceEnableAllButtons 삭제 - 불필요한 강제 활성화 제거
        }

        private void OnDisable()
        {
            Debug.Log("[MainScreen] OnDisable called");
            Bus<MemberTrainingStateChangedEvent>.OnEvent -= OnMemberTrainingStateChanged;
        }

        #endregion

        #region Init

        private void InitializeActionCounts()
        {
            // 기본값으로 초기화
            _memberActionCounts[MemberType.Bass] = false;
            _memberActionCounts[MemberType.Drums] = false;
            _memberActionCounts[MemberType.Guitar] = false;
            _memberActionCounts[MemberType.Piano] = false;
            _memberActionCounts[MemberType.Vocal] = false;
            
            // TrainingManager 상태 동기화
            SyncWithTrainingManager();
        }
        
        /// <summary>
        /// TrainingManager의 현재 상태와 동기화
        /// 씬 전환 후에도 훈련 완료 상태를 유지하기 위함
        /// </summary>
        private void SyncWithTrainingManager()
        {
            if (TrainingManager.Instance == null)
            {
                Debug.LogWarning("[MainScreen] TrainingManager.Instance is null - cannot sync");
                return;
            }
            
            foreach (var memberType in _memberActionCounts.Keys.ToList())
            {
                bool isTrained = TrainingManager.Instance.IsMemberTrained(memberType);
                _memberActionCounts[memberType] = isTrained;
                
                if (isTrained)
                {
                    Debug.Log($"[MainScreen] {memberType} is already trained - action blocked");
                }
            }
            
            // Busking 가능 여부 체크 - 한 명이라도 훈련했으면 불가능
            _isbuskingAvailable = !_memberActionCounts.Values.Any(acted => acted);
            
            Debug.Log($"[MainScreen] Synced with TrainingManager - Busking available: {_isbuskingAvailable}");
        }

        /// <summary>
        /// 로딩 완료 후 버튼 상태를 업데이트
        /// </summary>
        private void UpdateButtonsAfterLoad()
        {
            Debug.Log("[MainScreen] UpdateButtonsAfterLoad called");
            
            if (_currentUnit != null)
            {
                UpdateButton();
            }
            else
            {
                Debug.LogWarning("[MainScreen] Current unit is null, cannot update buttons");
            }
        }

        private async Task LoadUnitsAsync()
        {
            Debug.Log("[MainScreen] Loading units...");

            await GameResourceManager.Instance.LoadAllAsync<UnitDataSO>(unitLabel);
            
            _loadedUnits = new List<UnitDataSO>();
            var locations = await UnityEngine.AddressableAssets.Addressables
                .LoadResourceLocationsAsync(unitLabel, typeof(UnitDataSO)).Task;
            
            foreach (var location in locations)
            {
                var unit = GameResourceManager.Instance.Load<UnitDataSO>(location.PrimaryKey);
                if (unit != null)
                    _loadedUnits.Add(unit);
            }

            if (_loadedUnits == null || _loadedUnits.Count == 0)
            {
                Debug.LogError("[MainScreen] Failed to load units!");
                return;
            }

            Debug.Log($"[MainScreen] Loaded {_loadedUnits.Count} units");

            foreach (var unit in _loadedUnits)
            {
                if (unit == null)
                {
                    Debug.LogError("[MainScreen] Null unit found in loaded units!");
                    continue;
                }

                Debug.Log($"[MainScreen] Loading assets for unit: {unit.unitName}");
                await unit.LoadAssets();
            }

            UnitSelector = new UnitSelector();
            UnitSelector.Init(_loadedUnits);

            int waitCount = 0;
            while (StatManager.Instance != null && !StatManager.Instance.IsInitialized)
            {
                await Task.Yield();
                waitCount++;
                if (waitCount > 1000)
                {
                    Debug.LogError("[MainScreen] StatManager initialization timeout!");
                    break;
                }
            }

            Debug.Log("[MainScreen] StatManager initialized");

            _statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, StatManager.Instance);

            if (memberCarousel != null)
            {
                Debug.Log("[MainScreen] Initializing member carousel");
                memberCarousel.Init(_loadedUnits, OnMemberSelectedFromCarousel);
            }
            else
            {
                Debug.LogError("[MainScreen] MemberCarousel is not assigned!");
                if (_loadedUnits.Count > 0)
                {
                    Debug.Log("[MainScreen] Selecting first unit as fallback");
                    SelectUnit(_loadedUnits[0]);
                }
            }

            if (teamPracticeCompo != null)
            {
                teamPracticeCompo.CacheUnits(_loadedUnits);
            }

            Debug.Log("[MainScreen] LoadUnitsAsync completed");
        }

        #endregion

        #region Event Handling

        private void OnMemberTrainingStateChanged(MemberTrainingStateChangedEvent evt)
        {
            Debug.Log($"[MainScreen] Member training state changed: {evt.MemberType}");
            _memberActionCounts[evt.MemberType] = true;
            _isbuskingAvailable = false;
            UpdateButton();
        }

        /// <summary>
        /// 캐러셀에서 멤버가 선택되었을 때 호출되는 콜백
        /// </summary>
        private void OnMemberSelectedFromCarousel(UnitDataSO unit)
        {
            Debug.Log($"[MainScreen] Member selected from carousel: {(unit != null ? unit.unitName : "NULL")}");

            if (unit != null)
            {
                SelectUnit(unit);
            }
        }

        #endregion

        #region UI Interaction

        public void TeamButtonClicked()
        {
            teamPanel.gameObject.SetActive(true);
        }

        private void UpdateButton()
        {
            if (practiceBtns == null)
            {
                Debug.LogWarning("[MainScreen] practiceBtns is null");
                return;
            }

            if (practiceBtns.Count < 5)
            {
                Debug.LogWarning($"[MainScreen] practiceBtns count is {practiceBtns.Count}, expected at least 5");
                return;
            }

            if (_currentUnit == null)
            {
                Debug.LogWarning("[MainScreen] _currentUnit is null in UpdateButton");
                return;
            }

            bool isCurrentMemberActed = _memberActionCounts.ContainsKey(_currentUnit.memberType)
                ? _memberActionCounts[_currentUnit.memberType]
                : false;

            Debug.Log($"[MainScreen] UpdateButton - Current: {_currentUnit.memberType}, Acted: {isCurrentMemberActed}, Busking: {_isbuskingAvailable}");

            // 개인 훈련 버튼들 (0~3)
            for (int i = 0; i < 4; i++)
            {
                if (practiceBtns[i] != null)
                {
                    bool shouldEnable = !isCurrentMemberActed;
                    practiceBtns[i].interactable = shouldEnable;
                    Debug.Log($"[MainScreen] Button {i} ({practiceBtns[i].name}) set to {shouldEnable}");
                }
            }

            // 버스킹 버튼 (4)
            if (practiceBtns[4] != null)
            {
                practiceBtns[4].interactable = _isbuskingAvailable;
                Debug.Log($"[MainScreen] Busking button set to {_isbuskingAvailable}");
            }
        }

        #endregion

        #region Unit Selection

        private void SelectUnit(UnitDataSO unit)
        {
            Debug.Log($"[MainScreen] SelectUnit called with: {(unit != null ? unit.unitName : "NULL")}");

            if (unit == null)
            {
                Debug.LogError("[MainScreen] SelectUnit: unit is NULL!");
                return;
            }

            _currentUnit = unit;
            Debug.Log($"[MainScreen] Selecting unit: {unit.unitName} (MemberType: {unit.memberType})");

            if (personalPracticeCompo != null)
            {
                personalPracticeCompo.Init(unit, _statUIUpdater);
            }
            else
            {
                Debug.LogWarning("[MainScreen] personalPracticeCompo is null");
            }

            if (charterNameText != null)
            {
                Debug.Log($"[MainScreen] Setting name text to: {unit.unitName}");
                charterNameText.SetText(unit.unitName);
                charterNameText.ForceMeshUpdate();
                Debug.Log($"[MainScreen] Name text set successfully. Current text: {charterNameText.text}");
            }
            else
            {
                Debug.LogError("[MainScreen] charterNameText is NULL! Cannot set unit name.");
            }

            if (conditionText != null)
            {
                string conditionString = $"{unit.currentCondition}/{unit.maxCondition}";
                Debug.Log($"[MainScreen] Setting condition text to: {conditionString}");
                conditionText.SetText(conditionString);
            }
            else
            {
                Debug.LogWarning("[MainScreen] conditionText is null");
            }

            if (_statUIUpdater != null)
            {
                _statUIUpdater.UpdateAll(unit);
            }
            else
            {
                Debug.LogWarning("[MainScreen] _statUIUpdater is null");
            }

            LoadUnitSprite(unit);

            if (UnitSelector != null)
            {
                UnitSelector.SetCurrentUnit(unit);
            }

            UpdateButton();

            Debug.Log($"[MainScreen] SelectUnit completed for: {unit.unitName}");
        }

        private async void LoadUnitSprite(UnitDataSO unit)
        {
            if (unit == null)
            {
                Debug.LogError("[MainScreen] LoadUnitSprite: unit is null");
                return;
            }

            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
            {
                Debug.LogWarning($"[MainScreen] LoadUnitSprite: spriteAddressableKey is empty for {unit.unitName}");
                return;
            }

            if (characterIcon == null)
            {
                Debug.LogError("[MainScreen] LoadUnitSprite: characterIcon is null");
                return;
            }

            try
            {
                var sprite = await GameResourceManager.Instance.LoadAssetAsync<Sprite>(unit.spriteAddressableKey);

                if (sprite == null)
                {
                    Debug.LogError($"[MainScreen] Failed to load sprite for {unit.unitName} with key: {unit.spriteAddressableKey}");
                    return;
                }

                if (characterIcon != null)
                {
                    characterIcon.sprite = sprite;
                    characterIcon.color = Color.white;
                    Debug.Log($"[MainScreen] Sprite loaded successfully for {unit.unitName}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[MainScreen] Exception loading sprite for {unit.unitName}: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion

        /// <summary>
        /// 외부에서 특정 멤버를 선택하도록 요청할 때 사용
        /// </summary>
        public void SelectMemberByType(MemberType memberType)
        {
            Debug.Log($"[MainScreen] SelectMemberByType called: {memberType}");

            if (memberCarousel != null)
            {
                memberCarousel.SelectMember(memberType);
            }
            else
            {
                if (_loadedUnits != null)
                {
                    var unit = _loadedUnits.FirstOrDefault(u => u.memberType == memberType);
                    if (unit != null)
                    {
                        SelectUnit(unit);
                    }
                    else
                    {
                        Debug.LogError($"[MainScreen] Unit not found for MemberType: {memberType}");
                    }
                }
                else
                {
                    Debug.LogError("[MainScreen] _loadedUnits is null");
                }
            }
        }

        public void TurnStart()
        {
            Debug.Log("[MainScreen] TurnStart called - resetting actions");

            InitializeActionCounts();
            _isbuskingAvailable = true;

            UpdateButton();
        }
    }
}