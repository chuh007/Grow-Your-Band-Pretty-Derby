using System.Collections.Generic;
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

        [Header("Member Buttons (기본 화면)")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Components")]
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
        [SerializeField] private RestSelectCompo restSelectCompo;
        [SerializeField] private StatManager statManager;

        public UnitSelector UnitSelector { get; private set; }

        private StatUIUpdater _statUIUpdater;
        private List<UnitDataSO> _loadedUnits;
        
        private readonly Dictionary<MemberType, Button> _memberButtonMap = new();

        #region Unity LifeCycle

        private async void Start()
        {
            CacheMemberButtons();
            await LoadUnitsAsync();
            RefreshAllMemberButtons();
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

        private void CacheMemberButtons()
        {
            _memberButtonMap.Clear();

            foreach (var btn in memberButtons)
            {
                if (System.Enum.TryParse(btn.name, out MemberType type))
                {
                    _memberButtonMap[type] = btn;
                }
                else
                {
                    Debug.LogWarning($"Member 버튼 이름이 MemberType과 다름 : {btn.name}");
                }
            }
        }

        private async Task LoadUnitsAsync()
        {
            _loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);

            UnitSelector = new UnitSelector();
            UnitSelector.Init(_loadedUnits);

            _statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, statManager);

            teamPracticeCompo.CacheUnits(_loadedUnits);

            if (_loadedUnits.Count > 0)
            {
                SelectUnit(_loadedUnits[0]);
            }
        }

        #endregion

        #region Event Handling

        private void OnMemberTrainingStateChanged(MemberTrainingStateChangedEvent evt)
        {
            UpdateMemberButtonVisual(evt.MemberType);
        }

        #endregion

        #region Member Button Visual

        private void RefreshAllMemberButtons()
        {
            foreach (var kv in _memberButtonMap)
            {
                UpdateMemberButtonVisual(kv.Key);
            }
        }

        private void UpdateMemberButtonVisual(MemberType member)
        {
            if (!_memberButtonMap.TryGetValue(member, out var btn))
                return;

            bool isTrained = TrainingManager.Instance.IsMemberTrained(member);

            var image = btn.GetComponent<Image>();
            if (image != null)
            {
                image.color = isTrained ? Color.gray : Color.white;
            }
        }

        #endregion

        #region UI Interaction

        public void TeamButtonClicked()
        {
            teamPanel.gameObject.SetActive(true);
            teamPracticeCompo.CacheUnits(_loadedUnits);
        }

        public void MemberBtnClicked(string type)
        {
            if (!System.Enum.TryParse(type, out MemberType memberType))
                return;

            if (UnitSelector.TryGetUnit(type, out UnitDataSO unit) && unit != null)
            {
                SelectUnit(unit);
            }
            else
            {
                Debug.LogWarning($"No unit found for type: {type}");
            }
        }

        #endregion

        #region Unit Selection

        private void SelectUnit(UnitDataSO unit)
        {
            if (unit == null) return;

            personalPracticeCompo.Init(unit, _statUIUpdater);

            charterNameText.SetText(unit.unitName);
            conditionText.SetText($"{unit.currentCondition}/{unit.maxCondition}");

            _statUIUpdater.UpdateAll(unit);

            LoadUnitSprite(unit);
        }

        private async void LoadUnitSprite(UnitDataSO unit)
        {
            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
                return;
            
            restSelectCompo.CacheUnits(_loadedUnits);
            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
            characterIcon.sprite = sprite;
            characterIcon.color = Color.white;
        }

        #endregion
    }
}
