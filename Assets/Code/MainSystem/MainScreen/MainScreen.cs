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
        [SerializeField] private List<UnitDataSO> unitDataSOs = new List<UnitDataSO>();
        [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> characterStatNameTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> characterStatValueTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<Image> characterStatSprites = new List<Image>();
        [SerializeField] private Image characterIcon;

        public void MemberBtnClicked(string type)
        {
            foreach (var unit in unitDataSOs)
            {
                if (unit.memberType.ToString() == type)
                {
                    charterNameText.SetText(unit.unitName);
                    for (int i = 0;  i < characterStatNameTexts.Count; i++)
                    {
                        characterStatNameTexts[i].SetText(unitDataSOs[i].stats[i].statName);
                        characterStatValueTexts[i].SetText($"{unitDataSOs[i].stats[i].currentValue} / {unitDataSOs[i].stats[i].maxValue}");
                        //characterStatSprites[i].sprite = unit.stats[i].statIcon;
                        characterIcon.sprite = unit.unitImage;
                    }
                }
            }
        }
    }
}