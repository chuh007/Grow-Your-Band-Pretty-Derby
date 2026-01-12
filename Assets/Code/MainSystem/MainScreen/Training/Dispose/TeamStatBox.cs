using Code.MainSystem.MainScreen.MemberData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class TeamStatBox : MonoBehaviour
    {
        [SerializeField] private Image unitIcon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI harmonyText;

        public void Set(UnitDataSO unit, (string name, Sprite icon, int baseValue, int delta) harmonyStat)
        {
            unitIcon.sprite = unit.TeamStat.statIcon;
            nameText.text = unit.unitName;
            harmonyText.text = $"+{harmonyStat.delta}";
        }
    }

}