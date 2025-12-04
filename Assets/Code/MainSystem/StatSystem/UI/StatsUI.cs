using UnityEngine;
using UnityEngine.UI;
using Code.MainSystem.StatSystem.BaseStats;

namespace Code.MainSystem.StatSystem.UI
{

    public class StatsUI : MonoBehaviour, IUIElement<BaseStat>
    {
        [SerializeField] private Text statNameText;
        [SerializeField] private Text statValueText;
        [SerializeField] private Image statIcon;
        [SerializeField] private Slider statSlider;

        private BaseStat _stat;

        public void EnableFor(BaseStat baseStat)
        {
            _stat = baseStat;
            statNameText.text = _stat.StatName;
            statIcon.sprite = _stat.StatIcon;
            statSlider.minValue = 0f;
            statSlider.maxValue = 1f;
            Update();
        }

        public void Update()
        {
            statValueText.text = $"{_stat.CurrentValue.ToString()} / {_stat.MaxValue}";

            float ratio = Mathf.Clamp01((float)_stat.CurrentValue / _stat.MaxValue);
            statSlider.value = ratio;
        }
    }
}