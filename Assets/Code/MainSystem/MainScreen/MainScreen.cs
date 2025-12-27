using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
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

        [Header("Components")]
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private StatManager statManager;

        private UnitSelector _unitSelector;
        private StatUIUpdater _statUIUpdater;
        private List<UnitDataSO> _loadedUnits;
        private Button _currentSelectedButton;

        private async void Start()
        {
            await LoadUnitsAsync();
        }

        private async Task LoadUnitsAsync()
        {
            _loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);
            _unitSelector = new UnitSelector();
            _unitSelector.Init(_loadedUnits);

            _statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, statManager);

            if (_loadedUnits.Count > 0)
            {
                SelectUnit(_loadedUnits[0]);
            }
        }


        public void MemberBtnClicked(string type)
        {
            if (_unitSelector.TryGetUnit(type, out UnitDataSO unit) && unit != null)
            { 
                SelectUnit(unit); 
            }
            else
            {
                Debug.LogWarning($"No unit found for type: {type}");
            }
        }

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
            if (string.IsNullOrEmpty(unit.spriteAddressableKey)) return;

            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
            characterIcon.sprite = sprite;
            characterIcon.color = Color.white;
        }
    }
}
