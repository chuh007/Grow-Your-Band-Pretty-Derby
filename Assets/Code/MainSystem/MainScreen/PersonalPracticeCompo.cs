using System;
using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Events;
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
        [SerializeField] private TrainingSequenceController trainingSequenceController;

        private UnitDataSO _currentUnit;
        private float _currentCondition;
        private int _currentLesson = -1;
        private bool _showProbability = false;
        private bool _isSuccse = false;

        private void OnEnable()
        {
            Bus<StatUpgradeEvent>.OnEvent += HnaldeCanUpgarede;
        }

        private void HnaldeCanUpgarede(StatUpgradeEvent evt)
        {
            _isSuccse = evt.Upgrade;
        }

        private void OnDisable()
        {
            Bus<StatUpgradeEvent>.OnEvent -= HnaldeCanUpgarede;
        }

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
            HideAllArrows();
            HideAllProbabilityTexts();
        }

        public void CancelBtnClick()
        {
            ClearProbabilityTexts();
            HideAllArrows();
            HideAllProbabilityTexts();
            _showProbability = false;
        }

        public async void PracticeBtnClick(int index)
        {
            if (_currentUnit == null || index >= _currentUnit.personalPractices.Count) return;

            if (_currentLesson == index)
            {
                var p = _currentUnit.personalPractices[index];
                
                trainingSequenceController.gameObject.SetActive(true);
                
                if (trainingSequenceController != null)
                    await trainingSequenceController.PlayTrainingSequence(_isSuccse,p);
                
                float statGain = _isSuccse ? p.statIncrease : 0;
                Bus<PracticenEvent>.Raise(new PracticenEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    p.PracticeStatType,
                    _currentCondition,
                    statGain));

                _currentCondition -= p.StaminaReduction;
                _currentCondition = Mathf.Clamp(_currentCondition, 0f, _currentUnit.maxCondition);
                conditionText.SetText($"{_currentCondition}/{_currentUnit.maxCondition}");
            }
            else
            {
                UpdateLessonSelection(index);
                ShowCurrentProbability();
                ShowArrowForLesson(index);  
                ShowAllProbabilityTexts();  
            }
        }


        private void UpdateLessonSelection(int newIndex)
        {
            _currentLesson = newIndex;
        }

        private void ClearProbabilityTexts()
        {
            foreach (var t in probabilityTexts)
                t.SetText("");
        }

        private void ShowCurrentProbability()
        {
            if (_memberTypeIndexMap.TryGetValue((MemberType)(int)_currentUnit.memberType, out int idx) && idx < probabilityTexts.Count)
            {
                probabilityTexts[idx].SetText($"{_currentCondition}%");
            }
        }

        private void ShowArrowForLesson(int lessonIndex)
        {
            for (int i = 0; i < arrowObjs.Count; i++)
            {
                arrowObjs[i].gameObject.SetActive(i == lessonIndex);
            }
        }

        private void HideAllArrows()
        {
            foreach (var img in arrowObjs)
            {
                img.gameObject.SetActive(false);
            }
        }

        private void ShowAllProbabilityTexts()
        {
            foreach (var t in probabilityTexts)
            {
                t.gameObject.SetActive(true);
            }
        }

        private void HideAllProbabilityTexts()
        {
            foreach (var t in probabilityTexts)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}
