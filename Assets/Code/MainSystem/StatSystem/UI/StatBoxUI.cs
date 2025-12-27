using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.StatSystem.UI
{
    public class StatBox : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI valueText;

        public void Set(Sprite iconSprite, int baseValue, int delta)
        {
            icon.sprite = iconSprite;

            string deltaStr = delta == 0 ? "-" : (delta > 0 ? $"+{delta}" : $"{delta}");
            Color color = delta > 0 ? Color.green : delta < 0 ? Color.red : Color.gray;

            valueText.text = $"{baseValue} ({deltaStr})";
            valueText.color = color;
        }
    }
}