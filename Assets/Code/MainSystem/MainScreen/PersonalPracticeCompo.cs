using Code.MainSystem.MainScreen.MemberData;
using TMPro;
using UnityEngine;

namespace Code.MainSystem.MainScreen
{
    public class PersonalPracticeCompo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;
        public void ButtonLoader(UnitDataSO currentUnit)
        {
            if(currentUnit == null)
                return;
            
            lesson1Text.SetText(currentUnit.Lesson1TeXT);
            lesson2Text.SetText(currentUnit.Lesson2TeXT);
            
        }
    }
}