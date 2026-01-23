using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen.Training
{
    public class StatUpDownComment : MonoBehaviour
    {
        [SerializeField] private Image statIconImage;
        [SerializeField] private GameObject goUpImage;
        [SerializeField] private GameObject goDownImage;
        [SerializeField] private TextMeshProUGUI valueSucsseText; 
        [SerializeField] private TextMeshProUGUI valueFaillText; 

        [SerializeField] private Color upColor = Color.green;
        [SerializeField] private Color downColor = Color.red;

        public void Setup(StatChangeInfo stat)
        {
            if (statIconImage != null && stat.icon != null)
                statIconImage.sprite = stat.icon;

            bool isUp = stat.delta >= 0;
            
            if (goUpImage != null) goUpImage.SetActive(isUp);
            if (goDownImage != null) goDownImage.SetActive(!isUp);
            
            if (valueSucsseText != null) valueSucsseText.gameObject.SetActive(false);
            if (valueFaillText != null) valueFaillText.gameObject.SetActive(false);
            
            string sign = isUp ? "+" : "−"; 
            int absDelta = Mathf.Abs(stat.delta);
            string displayText = $"{sign}{absDelta}";

            if (isUp && valueSucsseText != null)
            {
                valueSucsseText.text = displayText;
                valueSucsseText.color = upColor;
                valueSucsseText.gameObject.SetActive(true);
            }
            else if (!isUp && valueFaillText != null)
            {
                valueFaillText.text = displayText;
                valueFaillText.color = downColor;
                valueFaillText.gameObject.SetActive(true);
            }

            Debug.Log($"[StatUpDownComment] delta: {stat.delta}, isUp: {isUp}, text: {displayText}");
        }
    }
}