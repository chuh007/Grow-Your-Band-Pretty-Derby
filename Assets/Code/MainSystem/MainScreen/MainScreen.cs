using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class MainScreen : MonoBehaviour
    {
        [Header("Addressables Keys/Labels")]
        [SerializeField] private string unitLabel = "Units";

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> statNameTexts;
        [SerializeField] private List<TextMeshProUGUI> statValueTexts;
        [SerializeField] private List<Image> statIcons;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private Image characterIcon;
        [SerializeField] private GameObject teamPanel;

        [Header("Member Carousel")]
        [SerializeField] private MemberCarousel memberCarousel;

        [Header("Components")]
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
        [SerializeField] private RestSelectCompo restSelectCompo;

        public UnitSelector UnitSelector { get; private set; }

        private StatUIUpdater _statUIUpdater;
        private List<UnitDataSO> _loadedUnits;

        #region Unity LifeCycle

        private async void Start()
        {
            try
            {
                await LoadUnitsAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[MainScreen] Error in Start: {e.Message}");
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

        private async Task LoadUnitsAsync()
        {
            _loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);
            
            if (_loadedUnits == null || _loadedUnits.Count == 0)
            {
                Debug.LogError("Failed to load units!");
                return;
            }

            // 각 유닛의 AssetReference 로드
            foreach (var unit in _loadedUnits)
            {
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
                    Debug.LogError("StatManager initialization timeout!");
                    break;
                }
            }

            _statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, StatManager.Instance);
            
            if (memberCarousel != null)
            {
                memberCarousel.Init(_loadedUnits, OnMemberSelectedFromCarousel);
            }
            else
            {
                Debug.LogError("MemberCarousel is not assigned!");
                if (_loadedUnits.Count > 0)
                {
                    SelectUnit(_loadedUnits[0]);
                }
            }
            
            if (teamPracticeCompo != null)
            {
                teamPracticeCompo.CacheUnits(_loadedUnits);
            }
            
            if (restSelectCompo != null)
            {
                restSelectCompo.CacheUnits(_loadedUnits);
            }
        }
        
        #endregion

        #region Event Handling

        private void OnMemberTrainingStateChanged(MemberTrainingStateChangedEvent evt)
        {
        }

        /// <summary>
        /// 캐러셀에서 멤버가 선택되었을 때 호출되는 콜백
        /// </summary>
        private void OnMemberSelectedFromCarousel(UnitDataSO unit)
        {
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

        #endregion

        #region Unit Selection

        private void SelectUnit(UnitDataSO unit)
        {
            if (unit == null)
            {
                Debug.LogError("SelectUnit: unit is NULL!");
                return;
            }

            if (personalPracticeCompo != null)
            {
                personalPracticeCompo.Init(unit, _statUIUpdater);
            }

            if (charterNameText != null)
                charterNameText.SetText(unit.unitName);

            if (conditionText != null)
                conditionText.SetText($"{unit.currentCondition}/{unit.maxCondition}");

            if (_statUIUpdater != null)
                _statUIUpdater.UpdateAll(unit);

            LoadUnitSprite(unit);
            
            if (UnitSelector != null)
            {
                UnitSelector.SetCurrentUnit(unit);
            }
        }

        private async void LoadUnitSprite(UnitDataSO unit)
        {
            if (unit == null)
            {
                Debug.LogError("LoadUnitSprite: unit is null");
                return;
            }
            
            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
            {
                Debug.LogWarning($"LoadUnitSprite: spriteAddressableKey is empty for {unit.unitName}");
                return;
            }
            
            if (characterIcon == null)
            {
                Debug.LogError("LoadUnitSprite: characterIcon is null");
                return;
            }
            
            try
            {
                if (GameManager.Instance == null)
                {
                    Debug.LogError("LoadUnitSprite: GameManager.Instance is null");
                    return;
                }
                
                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
                
                if (sprite == null)
                {
                    Debug.LogError($"Failed to load sprite for {unit.unitName} with key: {unit.spriteAddressableKey}");
                    return;
                }
                
                if (characterIcon != null)
                {
                    characterIcon.sprite = sprite;
                    characterIcon.color = Color.white;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Exception loading sprite for {unit.unitName}: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion

        /// <summary>
        /// 외부에서 특정 멤버를 선택하도록 요청할 때 사용
        /// </summary>
        public void SelectMemberByType(MemberType memberType)
        {
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
                }
            }
        }
    }
}