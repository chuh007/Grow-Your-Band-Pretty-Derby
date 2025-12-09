using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class PersonalPracticeCompo : MonoBehaviour
    {
        [SerializeField] private List<Image> arrowObjs = new List<Image>();
        [SerializeField] private List<TextMeshProUGUI> probabilitytexts = new List<TextMeshProUGUI>();
        [SerializeField] private TextMeshProUGUI conditionText = null;
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;
        private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
        private UnitDataSO _currentUnit;
        private int _currentLesson = -1;
        private float _currentCondition = 0;

        public void ButtonLoader(UnitDataSO currentUnit,List<TextMeshProUGUI> units)
        {
            
            foreach (var text in probabilitytexts)
            {
                text.SetText("");
            }

            foreach (var obj in arrowObjs)
            {
                Color color = obj.color;
                color.a = 0;
                obj.color = color;
            }
            if (_texts.Count == 0 && units != null)
            {
                _texts = units;
            }
            if(currentUnit == null)
                return;
            _currentUnit = currentUnit;
            _currentCondition = currentUnit.currentCondition;
            lesson1Text.SetText(currentUnit.personalPractices[2].PracticeStatName);
            lesson2Text.SetText(currentUnit.personalPractices[3].PracticeStatName);
        }

        public void PracticeBtnClick(int index)
        {
            if (_currentLesson == index)
            {
                Debug.Log($"훈련시작");
                Bus<PracticenEvent>.Raise(new PracticenEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    _currentUnit.personalPractices[index].PracticeStatType,
                    _currentCondition,
                    _currentUnit.personalPractices[index].statIncrease
                ));

                _currentCondition -= _currentUnit.personalPractices[index].StaminaReduction;
                _currentCondition = Mathf.Clamp(_currentCondition, 0, 100);
                conditionText.SetText($"{_currentCondition}/{_currentUnit.maxCondition}");
            }

            else
            {
                if (_currentLesson < 0)
                {
                    Color c = arrowObjs[index].color;
                    c.a = 1f;
                    arrowObjs[index].color = c;

                    _texts[index].SetText($"{_currentUnit.stats[index].statName}++");
                    _currentLesson = index;
                }
                else
                {

                    _texts[_currentLesson].SetText($"{_currentUnit.stats[_currentLesson].statName}");

                    Color oldC = arrowObjs[_currentLesson].color;
                    oldC.a = 0f;
                    arrowObjs[_currentLesson].color = oldC;

                    Color newC = arrowObjs[index].color;
                    newC.a = 1f;
                    arrowObjs[index].color = newC;

                    _texts[index].SetText($"{_currentUnit.stats[index].statName}++");

                    _currentLesson = index;
                }

                switch (_currentUnit.memberType)
                {
                    case MemberType.Bass:
                        probabilitytexts[0].SetText($"{_currentUnit.currentCondition}%");
                        break;
                    case MemberType.Drums:
                        probabilitytexts[1].SetText($"{_currentUnit.currentCondition}%");
                        break;
                    case MemberType.Guitar:
                        probabilitytexts[2].SetText($"{_currentUnit.currentCondition}%");
                        break;
                    case MemberType.Piano:
                        probabilitytexts[3].SetText($"{_currentUnit.currentCondition}%");
                        break;
                    case MemberType.Vocal:
                        probabilitytexts[4].SetText($"{_currentUnit.currentCondition}%");
                        break;
                }
            }
        }
    }
}