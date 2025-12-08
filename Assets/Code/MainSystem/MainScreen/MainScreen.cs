using System.Collections.Generic;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class MainScreen : MonoBehaviour
    {
        [SerializeField] private List<UnitDataSO> unitDataSOList = new List<UnitDataSO>();
        private Dictionary<string, UnitDataSO> unitDataDict = new Dictionary<string, UnitDataSO>();

        [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> characterStatNameTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> characterStatValueTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<Image> characterStatSprites = new List<Image>();
        [SerializeField] private Image characterIcon;
        [SerializeField] private PersonalPracticeCompo  personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo  teamPracticeCompo;

        private UnitDataSO currentUnit;
        private bool _isTeamPractice = false;

        private void Awake()
        {
            foreach (var unit in unitDataSOList)
            {
                var key = unit.memberType.ToString();
                if (!unitDataDict.ContainsKey(key))
                {
                    unitDataDict.Add(key, unit);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key detected for memberType: {key}");
                }
            }
        }

        public void PracticeClick()
        {
            personalPracticeCompo.ButtonLoader(currentUnit);
        }

        public void TeamClick()
        {
            _isTeamPractice = true;
        }


        public void MemberBtnClicked(string type)
        {
            if (unitDataDict.TryGetValue(type, out var unit))
            {
                currentUnit = unit;
                charterNameText.SetText(unit.unitName);

                for (int i = 0; i < 5; i++)
                {
                    characterStatNameTexts[i].SetText(unit.stats[i].statName);
                    characterStatValueTexts[i].SetText($"{unit.stats[i].currentValue} / {unit.stats[i].maxValue}");
                    //characterStatSprites[i].sprite = unit.stats[i].statIcon;
                }

                characterIcon.sprite = unit.unitImage;
                Color iconColor = characterIcon.color;
                iconColor.a = 1f;
                characterIcon.color = iconColor;
            }
            else
            {
                Debug.LogWarning($"No unit data found for type: {type}");
            }
        }
    }
}
