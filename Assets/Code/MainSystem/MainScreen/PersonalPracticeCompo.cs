using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Code.MainSystem.MainScreen
{
    public class PersonalPracticeCompo : MonoBehaviour
    {
        [SerializeField] private List<Image> arrowObjs;
        [SerializeField] private List<TextMeshProUGUI> probabilityTexts;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;

        private UnitDataSO _currentUnit;
        private float _currentCondition;
        private int _currentLesson = -1;
        private bool _showProbability = false;
        private readonly Dictionary<MemberType, int> _memberTypeIndexMap = new()
        {
            { MemberType.Bass, 0 },
            { MemberType.Drums, 1 },
            { MemberType.Guitar, 2 },
            { MemberType.Piano, 3 },
            { MemberType.Vocal, 4 }
        };

        public void ButtonLoader(UnitDataSO unit, List<TextMeshProUGUI> statTexts)
        {
            _currentUnit = unit;
            _currentCondition = unit.currentCondition;

            lesson1Text.SetText(unit.personalPractices.Count > 2 ? unit.personalPractices[2].PracticeStatName : "");
            lesson2Text.SetText(unit.personalPractices.Count > 3 ? unit.personalPractices[3].PracticeStatName : "");

            ClearProbabilityTexts();
        }

        public void CancelBtnClick()
        {
            ClearProbabilityTexts();
            SetAllArrowAlphas(0f);
            _showProbability = false;
        }

        public void PracticeBtnClick(int index)
        {
            if (_currentUnit == null || index >= _currentUnit.personalPractices.Count) return;

            if (_currentLesson == index)
            {
                var p = _currentUnit.personalPractices[index];
                Bus<PracticenEvent>.Raise(new PracticenEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    p.PracticeStatType,
                    _currentCondition,
                    p.statIncrease));

                _currentCondition -= p.StaminaReduction;
                _currentCondition = Mathf.Clamp(_currentCondition, 0f, _currentUnit.maxCondition);
                conditionText.SetText($"{_currentCondition}/{_currentUnit.maxCondition}");
            }
            else
            {
                UpdateLessonSelection(index);
                ShowCurrentProbability();
            }
        }

        private void UpdateLessonSelection(int newIndex)
        {
            _currentLesson = newIndex;
            // Arrow / UI feedback
        }

        private void ClearProbabilityTexts()
        {
            foreach (var t in probabilityTexts) t.SetText("");
        }

        private void ShowCurrentProbability()
        {
            if (_memberTypeIndexMap.TryGetValue((MemberType)(int)_currentUnit.memberType, out int idx) && idx < probabilityTexts.Count)
            {
                probabilityTexts[idx].SetText($"{_currentCondition}%");
            }

        }

        private void SetAllArrowAlphas(float a)
        {
            foreach (var img in arrowObjs)
            {
                Color c = img.color;
                c.a = a;
                img.color = c;
            }
        }
    }
}
