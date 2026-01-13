using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MainSystem.Turn.UI
{
    public class DayOfWeekUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI dayOfWeekText;
        
        public RectTransform RectTransform { get; private set; }
        
        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public void SetDayText(int dayCount) => dayText.SetText(dayCount.ToString());
        public void SetDayOfWeekText(string dayOfWeek) => dayOfWeekText.SetText(dayOfWeek);
    }
}