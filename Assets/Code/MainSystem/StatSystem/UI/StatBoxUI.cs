using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.StatSystem.UI
{
    public class StatBox : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Set(string name,Sprite iconSprite, int baseValue, int delta)
        {
            nameText.text = name;
            icon.sprite = iconSprite;

            string deltaStr = delta == 0 ? "-" : (delta > 0 ? $"+{delta}" : $"{delta}");
            Color color = delta > 0 ? Color.green : delta < 0 ? Color.red : Color.gray;

            valueText.text = $"{baseValue} ({deltaStr})";
            valueText.color = color;
        }
    }
}