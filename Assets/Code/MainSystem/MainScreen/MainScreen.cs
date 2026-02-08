using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
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
            Debug.Log("[MainScreen] Start called");
            try
            {
                InitializeActionCounts();
                await LoadUnitsAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[MainScreen] Error in Start: {e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        private void OnEnable()
        {
            Bus<MemberTrainingStateChangedEvent>.OnEvent += OnMemberTrainingStateChanged;
        }

        private void OnDisable()
        {
            Bus<MemberTrainingStateChangedEvent>.OnEvent -= OnMemberTrainingStateChanged;
        }

        #endregion

        #region Init

        private void InitializeActionCounts()
        {
            _memberActionCounts[MemberType.Bass] = false;
            _memberActionCounts[MemberType.Drums] = false;
            _memberActionCounts[MemberType.Guitar] = false;
            _memberActionCounts[MemberType.Piano] = false;
            _memberActionCounts[MemberType.Vocal] = false;
        }

        private async Task LoadUnitsAsync()
        {
            Debug.Log("[MainScreen] Loading units...");

            _loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);

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

            bool isCurrentMemberActed = _memberActionCounts[_currentUnit.memberType];

             practiceBtns[0].interactable = !isCurrentMemberActed;
            practiceBtns[1].interactable = !isCurrentMemberActed;
             practiceBtns[2].interactable = !isCurrentMemberActed;
            practiceBtns[3].interactable = !isCurrentMemberActed;
            practiceBtns[4].interactable = _isbuskingAvailable;
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
                if (GameManager.Instance == null)
                {
                    Debug.LogError("[MainScreen] LoadUnitSprite: GameManager.Instance is null");
                    return;
                }

                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);

                if (sprite == null)
                {
                    Debug.LogError(
                        $"[MainScreen] Failed to load sprite for {unit.unitName} with key: {unit.spriteAddressableKey}");
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
                Debug.LogError(
                    $"[MainScreen] Exception loading sprite for {unit.unitName}: {e.Message}\n{e.StackTrace}");
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
            
            if (practiceBtns != null)
            {
                for (int i = 0; i < practiceBtns.Count; i++)
                {
                    if (practiceBtns[i] != null)
                    {
                        practiceBtns[i].interactable = true;
                        Debug.Log($"[MainScreen] Force enabled button {i}: {practiceBtns[i].name}");
                    }
                }
            }
    
            UpdateButton();
        }
    }
}