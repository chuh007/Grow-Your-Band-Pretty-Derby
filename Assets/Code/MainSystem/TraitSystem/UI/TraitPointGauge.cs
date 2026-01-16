using UnityEngine;
using Code.MainSystem.TraitSystem.Interface;
using TMPro;

namespace Code.MainSystem.TraitSystem.UI
{
    public class TraitPointGauge : MonoBehaviour, IUIElement<int, int>
    {
        [SerializeField] private TextMeshProUGUI pointText;
        
        public void EnableFor(int totalPoint, int maxPoint)
        {
            pointText.SetText($"{totalPoint} / {maxPoint}");
            pointText.color = GetTextColor(totalPoint, maxPoint);
        }
        
        public void Disable()
        {
            pointText.SetText("");
            pointText.color = Color.black;
        }
        
        private Color GetTextColor(int totalPoint, int maxPoint)
            => totalPoint > maxPoint ? Color.red : Color.black;
    }
}