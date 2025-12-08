using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.StatSystem.BaseStats;
using TMPro;

namespace Code.MainSystem.StatSystem.UI
{

    public class StatUI : MonoBehaviour, IUIElement<BaseStat>
    {
        [SerializeField] private TextMeshProUGUI statNameText;
        [SerializeField] private TextMeshProUGUI statValueText;
        [SerializeField] private Image statIcon;
        
        private BaseStat _stat;
        
        public void EnableFor(BaseStat stat)
        {
            _stat = stat;
            gameObject.SetActive(stat != null);
            
            if (_stat != null)
            {
                statNameText.text = _stat.StatName;
                statIcon.sprite = _stat.StatIcon;
                UpdateUI();
            }
        }
        
        public void UpdateUI()
        {
            if (_stat != null)
            {
                statValueText.text = _stat.CurrentValue.ToString();
            }
        }
    }
}