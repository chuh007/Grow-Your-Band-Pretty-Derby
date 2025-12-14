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
        [SerializeField] private List<Image> arrowObjs;
        [SerializeField] private List<TextMeshProUGUI> probabilityTexts;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;

        private List<TextMeshProUGUI> _statTexts = new List<TextMeshProUGUI>();
        private UnitDataSO _currentUnit;
        private int _currentLesson = -1;
        private float _currentCondition = 100f;
        private bool _showProbability = false;

        private readonly Dictionary<MemberType, int> _memberTypeIndexMap = new Dictionary<MemberType, int>
        {
            { MemberType.Bass, 0 },
            { MemberType.Drums, 1 },
            { MemberType.Guitar, 2 },
            { MemberType.Piano, 3 },
            { MemberType.Vocal, 4 }
        };

        public void ButtonLoader(UnitDataSO currentUnit, List<TextMeshProUGUI> statTexts)
        {
            if (statTexts != null && _statTexts.Count == 0)
                _statTexts = statTexts;

            if (currentUnit == null)
                return;

            _currentUnit = currentUnit;
            _currentCondition = currentUnit.currentCondition;

            UpdateLessonTexts();
            ClearProbabilityTexts();

            if (_showProbability)
                ShowCurrentProbability();
        }

        public void CancelBtnClick()
        {
            ClearProbabilityTexts();
            SetAllArrowAlphas(0f);
            _showProbability = false;
        }

        public void PracticeBtnClick(int index)
        {
            if (_currentUnit == null || index >= _currentUnit.personalPractices.Count)
                return;

            _showProbability = true;

            if (_currentLesson == index)
            {
                Debug.Log("훈련시작");

                var practice = _currentUnit.personalPractices[index];

                Bus<PracticenEvent>.Raise(new PracticenEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    practice.PracticeStatType,
                    _currentCondition,
                    practice.statIncrease
                ));

                _currentCondition -= practice.StaminaReduction;
                _currentCondition = Mathf.Clamp(_currentCondition, 0f, 100f);
                conditionText.SetText($"{_currentCondition}/{_currentUnit.maxCondition}");
            }
            else
            {
                UpdateLessonSelection(index);
                ShowCurrentProbability();
            }
        }
        
        private void UpdateLessonTexts()
        {
            if (_currentUnit.personalPractices.Count >= 4)
            {
                lesson1Text.SetText(_currentUnit.personalPractices[2].PracticeStatName);
                lesson2Text.SetText(_currentUnit.personalPractices[3].PracticeStatName);
            }
        }

        private void UpdateLessonSelection(int newIndex)
        {
            if (_currentLesson >= 0 && _currentLesson < _statTexts.Count)
            {
                _statTexts[_currentLesson].SetText(_currentUnit.stats[_currentLesson].statName);
                SetArrowAlpha(_currentLesson, 0f);
            }

            if (newIndex < _statTexts.Count)
            {
                _statTexts[newIndex].SetText($"{_currentUnit.stats[newIndex].statName}++");
                SetArrowAlpha(newIndex, 1f);
            }

            _currentLesson = newIndex;
        }

        private void SetArrowAlpha(int index, float alpha)
        {
            if (index >= 0 && index < arrowObjs.Count)
            {
                var color = arrowObjs[index].color;
                color.a = alpha;
                arrowObjs[index].color = color;
            }
        }

        private void SetAllArrowAlphas(float alpha)
        {
            foreach (var arrow in arrowObjs)
            {
                var color = arrow.color;
                color.a = alpha;
                arrow.color = color;
            }
        }

        private void ClearProbabilityTexts()
        {
            foreach (var text in probabilityTexts)
                text.SetText("");
        }

        private void ShowCurrentProbability()
        {
            if (_currentUnit == null)
                return;

            ClearProbabilityTexts();

            if (_memberTypeIndexMap.TryGetValue(_currentUnit.memberType, out int index) && index < probabilityTexts.Count)
            {
                probabilityTexts[index].SetText($"{_currentCondition}%");
            }
        }
    }
}
