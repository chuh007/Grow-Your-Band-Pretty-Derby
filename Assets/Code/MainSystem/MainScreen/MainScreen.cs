using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class MainScreen : MonoBehaviour
    {
        [Header("Addressables Keys/Labels")]
        [SerializeField] private string unitLabel = "Units"; // Addressables label for all units

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

        private UnitSelector unitSelector;
        private StatUIUpdater statUIUpdater;
        private List<UnitDataSO> loadedUnits;

        private async void Start()
        {
            await LoadUnitsAsync();
        }

        private async Task LoadUnitsAsync()
        {
            loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);
            unitSelector = new UnitSelector();
            unitSelector.Init(loadedUnits);

            statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, statManager);

            if (loadedUnits.Count > 0)
            {
                SelectUnit(loadedUnits[0]);
            }
        }

        public void MemberBtnClicked(string type)
        {
            if (unitSelector.TryGetUnit(type, out UnitDataSO unit))
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
            personalPracticeCompo.ButtonLoader(unit, statNameTexts);

            charterNameText.SetText(unit.unitName);
            conditionText.SetText($"{unit.currentCondition}/{unit.maxCondition}");

            statUIUpdater.UpdateAll(unit);

            LoadUnitSprite(unit);
        }

        private async void LoadUnitSprite(UnitDataSO unit)
        {
            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
                return;

            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
            characterIcon.sprite = sprite;
            characterIcon.color = Color.white;
        }
    }
}
