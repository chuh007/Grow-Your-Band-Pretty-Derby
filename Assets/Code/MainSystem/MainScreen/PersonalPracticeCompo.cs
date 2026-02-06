using System;
using System.Collections.Generic;
using System.Globalization;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.Etc;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.MainScreen
{
    public class PersonalPracticeCompo : MonoBehaviour
    {
        [Serializable]
        public class UnitHealthBars
        {
            public MemberType memberType;
            public HealthBar healthBar;
        }

        [Header("UI")] [SerializeField] private HealthBar healthBar;
        [SerializeField] private PersonalPracticeSequencePlayer personalTrainingSequenceController;
        [SerializeField] private List<Image> arrowObjs;
        [SerializeField] private TextMeshProUGUI probabilityText;
        [SerializeField] private List<Button> practiceButtons;
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;
        
        private UnitDataSO _currentUnit;
        private float _currentCondition;
        private float _previewDamage;
        private int _selectedPracticeIndex = -1;

        private StatUIUpdater _statUIUpdater;

        private readonly Dictionary<MemberType, int> _memberTypeIndexMap = new()
        {
            { MemberType.Bass, 0 },
            { MemberType.Drums, 1 },
            { MemberType.Guitar, 2 },
            { MemberType.Piano, 3 },
            { MemberType.Vocal, 4 }
        };

        public void ResetPreview()
        {
            _selectedPracticeIndex = -1;
            _previewDamage = 0f;
            
            healthBar.SetHealth(_currentCondition, _currentUnit.maxCondition);

            _statUIUpdater.UpdateAll(_currentUnit);

            HideAllArrows();
            HideProbabilityText();
        }

        #region Init

        public void Init(UnitDataSO unit, StatUIUpdater statUIUpdater)
        {
            _currentUnit = unit;
            _statUIUpdater = statUIUpdater;
            _currentCondition = unit.currentCondition;

            healthBar.SetHealth(_currentCondition, unit.maxCondition);

            _statUIUpdater.UpdateAll(unit);

            lesson1Text.text = unit.stats.Count > 2 ? unit.stats[2].statName : "";
            lesson2Text.text = unit.stats.Count > 3 ? unit.stats[3].statName : "";

            _selectedPracticeIndex = -1;

            HideAllArrows();
            HideProbabilityText();

            UpdateButtonsState();
        }

        #endregion

        #region Practice Button

        public async void PracticeBtnClick(int index)
        {
            if (_currentUnit == null) return;
            if (index < 0 || index >= _currentUnit.personalPractices.Count) return;
            if (TrainingManager.Instance.IsMemberTrained(_currentUnit.memberType)) return;

            var practice = _currentUnit.personalPractices[index];

            if (_selectedPracticeIndex == index)
            {
                bool success 
                    = StatManager.Instance.PredictMemberPractice(_currentCondition, TraitManager.Instance.GetHolder(_currentUnit.memberType));

                Bus<PracticenEvent>.Raise(new PracticenEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    practice.PracticeStatType,
                    _currentCondition,
                    success ? practice.statIncrease : 0
                ));

                float realDamage = StatManager.Instance
                    .GetConditionHandler()
                    .ModifyConditionCost(_currentUnit.memberType, practice.StaminaReduction);
                
                Debug.Log(realDamage.ToString(CultureInfo.InvariantCulture));

                _currentCondition = Mathf.Clamp(
                    _currentCondition - realDamage,
                    0,
                    _currentUnit.maxCondition);
                
                _currentUnit.currentCondition = _currentCondition;
                healthBar.ApplyHealth(realDamage);

                TrainingManager.Instance.MarkMemberTrained(_currentUnit.memberType);

                _selectedPracticeIndex = -1;
                _statUIUpdater.UpdateAll(_currentUnit);
                
                personalTrainingSequenceController.gameObject.SetActive(true);
                await personalTrainingSequenceController.Play(
                    _currentUnit,
                    success,
                    practice,
                    _currentCondition,              
                    _currentUnit.teamStat,            
                    StatManager.Instance.GetTeamStat(StatType.TeamHarmony).CurrentValue
                );


                
                healthBar.SetHealth(_currentCondition, _currentUnit.maxCondition);

                HideAllArrows();
                HideProbabilityText();
                UpdateButtonsState();
                return;
            }

            _selectedPracticeIndex = index;
            _previewDamage = practice.StaminaReduction;
            
            healthBar.PrevieMinusHealth(_previewDamage);

            _statUIUpdater.PreviewStat(_currentUnit, practice.PracticeStatType, practice.statIncrease);

            ShowArrow(index);
            ShowProbability();
        }

        #endregion

        #region UI Helpers

        private void UpdateButtonsState()
        {
            bool trained = TrainingManager.Instance.IsMemberTrained(_currentUnit.memberType);

            foreach (var btn in practiceButtons)
            {
                btn.interactable = !trained;
                btn.image.color = trained ? Color.gray : Color.white;
            }
        }

        private void ShowArrow(int index)
        {
            for (int i = 0; i < arrowObjs.Count; i++)
            {
                arrowObjs[i].gameObject.SetActive(i == index);
            }
        }

        private void HideAllArrows()
        {
            foreach (var arrow in arrowObjs)
            {
                arrow.gameObject.SetActive(false);
            }
        }

        private void ShowProbability()
        {
            probabilityText.gameObject.SetActive(true);
            probabilityText.SetText($"성공확률: {Mathf.FloorToInt(_currentCondition)}%");
        }

        private void HideProbabilityText()
        {
            probabilityText.gameObject.SetActive(false);
        }

        #endregion
    }
}
