using System.Collections.Generic;
using Code.Core.Bus;
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
        [Header("Data")]
        [SerializeField] private List<UnitDataSO> unitDataSOList;
        private UnitSelector unitSelector;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> statNameTexts;
        [SerializeField] private List<TextMeshProUGUI> statValueTexts;
        [SerializeField] private List<Image> statIcons;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private Image characterIcon;

        [Header("Components")]
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
        [SerializeField] private StatManager statManager;

        private StatUIUpdater statUIUpdater;

        private void Awake()
        {
            unitSelector = new UnitSelector(unitDataSOList);
            statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, statManager);

            Bus<StatUpgradeEvent>.OnEvent += HandleEvent;
        }

        private void OnDestroy()
        {
            Bus<StatUpgradeEvent>.OnEvent -= HandleEvent;
        }

        private void HandleEvent(StatUpgradeEvent evt)
        {
            if (evt.Upgrade && unitSelector.CurrentUnit != null)
            {
                statUIUpdater.UpdateStatValues(unitSelector.CurrentUnit);
            }
        }

        public void TeamClick()
        {
        }

        public void MemberBtnClicked(string type)
        {
            if (unitSelector.TryGetUnit(type, out UnitDataSO unit))
            {
                personalPracticeCompo.ButtonLoader(unit, statNameTexts);

                charterNameText.SetText(unit.unitName);
                conditionText.SetText($"{unit.currentCondition}/{unit.maxCondition}");

                statUIUpdater.UpdateAll(unit);

                characterIcon.sprite = unit.unitImage;
                characterIcon.color = new Color(1, 1, 1, 1);
            }
            else
            {
                Debug.LogWarning($"No unit data found for type: {type}");
            }
        }
    }
}
