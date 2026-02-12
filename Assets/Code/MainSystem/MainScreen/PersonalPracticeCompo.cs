using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.EncounterEvents;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.Etc;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

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

        [Header("UI")] 
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private PersonalPracticeSequencePlayer personalTrainingSequenceController;
        [SerializeField] private List<Image> arrowObjs;
        [SerializeField] private TextMeshProUGUI probabilityText;
        [SerializeField] private TextMeshProUGUI practiceNameText;
        [SerializeField] private List<Button> practiceButtons;
        [SerializeField] private TextMeshProUGUI lesson1Text;
        [SerializeField] private TextMeshProUGUI lesson2Text;
        
        [Header("Panel Animation")]
        [SerializeField] private Button enterPersonalPracticeButton;
        [SerializeField] private TextMeshProUGUI enterPracticeText;
        [SerializeField] private GameObject practicePanel; 
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private float expandHeight = 200f;
        
        private UnitDataSO _currentUnit;
        private float _currentCondition;
        private float _previewDamage;
        private int _selectedPracticeIndex = -1;
        private bool _isPanelExpanded = false;

        private StatUIUpdater _statUIUpdater;
        private RectTransform _practicePanelRect;
        private Vector2 _panelOriginalSize;
        private Vector2 _panelOriginalPosition;

        private readonly Dictionary<MemberType, int> _memberTypeIndexMap = new()
        {
            { MemberType.Bass, 0 },
            { MemberType.Drums, 1 },
            { MemberType.Guitar, 2 },
            { MemberType.Piano, 3 },
            { MemberType.Vocal, 4 }
        };

        private void Awake()
        {
            if (enterPersonalPracticeButton != null)
            {
                enterPersonalPracticeButton.onClick.AddListener(OnTogglePersonalPractice);
            }
            
            if (practicePanel != null)
            {
                _practicePanelRect = practicePanel.GetComponent<RectTransform>();
                if (_practicePanelRect != null)
                {
                    _panelOriginalSize = _practicePanelRect.sizeDelta;
                    _panelOriginalPosition = _practicePanelRect.anchoredPosition;
                }
                
                foreach (var btn in practiceButtons)
                {
                    if (btn != null)
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnTogglePersonalPractice()
        {
            if (_isPanelExpanded)
            {
                CollapsePanel();
            }
            else
            {
                ExpandPanel();
            }
        }
        private void ExpandPanel()
        {
            if (_practicePanelRect == null) return;
    
            _isPanelExpanded = true;
            enterPracticeText.SetText("닫기");
    
            Debug.Log("[PersonalPracticeCompo] Expanding panel");
            
            _practicePanelRect.anchoredPosition = new Vector2(105, _practicePanelRect.anchoredPosition.y);
    
            foreach (var btn in practiceButtons)
            {
                if (btn != null)
                {
                    btn.gameObject.SetActive(true);
                    btn.GetComponent<RectTransform>().localScale = Vector3.zero;
                }
            }
    
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_practicePanelRect);
    
            Vector2 targetSize = _practicePanelRect.sizeDelta;
    
            _practicePanelRect.sizeDelta = _panelOriginalSize;
            
            Sequence expandSequence = DOTween.Sequence();
            expandSequence.Append(_practicePanelRect.DOSizeDelta(targetSize, animationDuration).SetEase(Ease.OutCubic));
    
            expandSequence.OnComplete(() =>
            {
                ShowPracticeButtons();
            });
        }

        private void CollapsePanel()
        {
            if (_practicePanelRect == null) return;
    
            _isPanelExpanded = false;
            enterPracticeText.SetText("개인연습");
    
            Debug.Log("[PersonalPracticeCompo] Collapsing panel");
    
            HidePracticeButtons();
            
            Sequence collapseSequence = DOTween.Sequence();
            collapseSequence.Append(_practicePanelRect.DOSizeDelta(_panelOriginalSize, animationDuration).SetEase(Ease.InCubic));
    
            collapseSequence.OnComplete(() =>
            {
                ResetPreview();
            });
        }

        private void ShowPracticeButtons()
        {
            if (practiceButtons == null) return;
            
            for (int i = 0; i < practiceButtons.Count; i++)
            {
                if (practiceButtons[i] != null)
                {
                    var btn = practiceButtons[i];
                    btn.gameObject.SetActive(true);

                    var canvasGroup = btn.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = btn.gameObject.AddComponent<CanvasGroup>();
                    }
                    
                    canvasGroup.alpha = 0f;
                    canvasGroup.DOFade(1f, animationDuration * 0.5f).SetDelay(i * 0.05f);
                    
                    var rectTransform = btn.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.zero;
                        rectTransform.DOScale(Vector3.one, animationDuration)
                            .SetEase(Ease.OutBack)
                            .SetDelay(i * 0.05f);
                    }
                }
            }
        }

        private void HidePracticeButtons()
        {
            if (practiceButtons == null) return;
            
            foreach (var btn in practiceButtons)
            {
                if (btn != null)
                {
                    var canvasGroup = btn.GetComponent<CanvasGroup>();
                    var rectTransform = btn.GetComponent<RectTransform>();
                    
                    if (canvasGroup != null && rectTransform != null)
                    {
                        canvasGroup.DOFade(0f, animationDuration * 0.3f);
                        rectTransform.DOScale(Vector3.zero, animationDuration * 0.3f)
                            .OnComplete(() =>
                            {
                                btn.gameObject.SetActive(false);
                                rectTransform.localScale = Vector3.one; 
                            });
                    }
                    else
                    {
                        btn.gameObject.SetActive(false);
                    } 
                }
            }
        }

        public void ResetPreview()
        {
            _selectedPracticeIndex = -1;
            _previewDamage = 0f;
            
            if (healthBar != null && _currentUnit != null)
            {
                healthBar.SetHealth(_currentCondition, _currentUnit.maxCondition);
            }

            if (_statUIUpdater != null && _currentUnit != null)
            {
                _statUIUpdater.UpdateAll(_currentUnit);
            }

            HideAllArrows();
            HideProbabilityText();
            practiceNameText.gameObject.SetActive(false);
        }

        #region Init

        public void Init(UnitDataSO unit, StatUIUpdater statUIUpdater)
        {
            Debug.Log($"[PersonalPracticeCompo] Init called for {(unit != null ? unit.unitName : "NULL")}");
            
            if (unit == null)
            {
                Debug.LogError("[PersonalPracticeCompo] Init: unit is NULL!");
                return;
            }
            
            if (statUIUpdater == null)
            {
                Debug.LogError("[PersonalPracticeCompo] Init: statUIUpdater is NULL!");
                return;
            }

            _currentUnit = unit;
            _statUIUpdater = statUIUpdater;
            _currentCondition = unit.currentCondition;

            if (healthBar != null)
            {
                healthBar.SetHealth(_currentCondition, unit.maxCondition);
            }
            else
            {
                Debug.LogError("[PersonalPracticeCompo] healthBar is NULL!");
            }

            _statUIUpdater.UpdateAll(unit);

            if (lesson1Text != null)
            {
                lesson1Text.text = unit.stats != null && unit.stats.Count > 2 ? unit.stats[2].statName : "";
            }
            else
            {
                Debug.LogWarning("[PersonalPracticeCompo] lesson1Text is not assigned in Inspector");
            }
            
            if (lesson2Text != null)
            {
                lesson2Text.text = unit.stats != null && unit.stats.Count > 3 ? unit.stats[3].statName : "";
            }
            else
            {
                Debug.LogWarning("[PersonalPracticeCompo] lesson2Text is not assigned in Inspector");
            }

            _selectedPracticeIndex = -1;
            
            if (_isPanelExpanded)
            {
                CollapsePanel();
            }

            HideAllArrows();
            HideProbabilityText();
            
            Debug.Log($"[PersonalPracticeCompo] Init completed for {unit.unitName}");
        }

        #endregion

        #region Practice Button

        public async void PracticeBtnClick(int index)
        {
            if (!_isPanelExpanded)
            {
                Debug.LogWarning("[PersonalPracticeCompo] Cannot select practice - panel not expanded");
                return;
            }
            
            if (_currentUnit == null) 
            {
                Debug.LogError("[PersonalPracticeCompo] PracticeBtnClick: _currentUnit is NULL!");
                return;
            }
            
            if (index < 0 || index >= _currentUnit.personalPractices.Count)
            {
                Debug.LogError($"[PersonalPracticeCompo] Invalid practice index: {index}");
                return;
            }
            
            if (TrainingManager.Instance.IsMemberTrained(_currentUnit.memberType))
            {
                Debug.Log($"[PersonalPracticeCompo] Member {_currentUnit.memberType} already trained");
                return;
            }

            var practice = _currentUnit.personalPractices[index];
            var holder = TraitManager.Instance.GetHolder(_currentUnit.memberType);
            
            if (_selectedPracticeIndex == index)
            {
                var disciplined = holder.GetModifiers<IDisciplinedLifestyle>().FirstOrDefault();
                var bufferedEffects = holder.GetModifiers<IGrooveRestoration>().FirstOrDefault();
                var consecutive = holder.GetModifiers<IConsecutiveActionModifier>().FirstOrDefault();
                var additionalAction = holder.GetModifiers<IAdditionalActionProvider>().FirstOrDefault();
                
                if (bufferedEffects != null) 
                    bufferedEffects.IsBuffered = false;
                
                bool success = StatManager.Instance.PredictMemberPractice(
                    _currentCondition, 
                    TraitManager.Instance.GetHolder(_currentUnit.memberType)
                );
                
                float increaseValue = success ? practice.statIncrease : 0;

                if (disciplined != null)
                {
                    increaseValue += disciplined.CheckPractice(practice.PracticeStatType);
                }
                
                if(consecutive != null)
                    increaseValue *= consecutive.GetSuccessBonus(practice.PracticeStatType.ToString());
                
                Bus<PracticeEvent>.Raise(new PracticeEvent(
                    PracticenType.Personal,
                    _currentUnit.memberType,
                    practice.PracticeStatType,
                    success,
                    increaseValue
                ));
                
                float realDamage = StatManager.Instance
                    .GetConditionHandler()
                    .ModifyConditionCost(_currentUnit.memberType, practice.StaminaReduction);
                
                Debug.Log($"[PersonalPracticeCompo] Real damage: {realDamage}");

                _currentCondition = Mathf.Clamp(
                    _currentCondition - realDamage,
                    0,
                    _currentUnit.maxCondition);
                
                _currentUnit.currentCondition = _currentCondition;
                
                if (healthBar != null)
                {
                    healthBar.ApplyHealth(realDamage);
                }

                if (additionalAction != null)
                {
                    float rand = Random.Range(0f, 100f);
                    if (rand < additionalAction.AdditionalActionChance)
                    {
                        TrainingManager.Instance.RestoreMemberAction(_currentUnit.memberType, 1);
                    }
                }
                
                TrainingManager.Instance.MarkMemberTrained(_currentUnit.memberType);

                _selectedPracticeIndex = -1;
                _statUIUpdater.UpdateAll(_currentUnit);
                
                if (personalTrainingSequenceController != null)
                {
                    personalTrainingSequenceController.gameObject.SetActive(true);
                    await personalTrainingSequenceController.Play(
                        _currentUnit,
                        success,
                        practice,
                        _currentCondition,              
                        _currentUnit.teamStat,            
                        StatManager.Instance.GetTeamStat(StatType.TeamHarmony).CurrentValue
                    );
                }
                else
                {
                    Debug.LogWarning("[PersonalPracticeCompo] personalTrainingSequenceController is NULL!");
                }

                if (healthBar != null)
                {
                    healthBar.SetHealth(_currentCondition, _currentUnit.maxCondition);
                }

                HideAllArrows();
                HideProbabilityText();
                
                CollapsePanel();
                Bus<TrainingEndEncounterEvent>.Raise(new TrainingEndEncounterEvent(practice));
                return;
            }

            _selectedPracticeIndex = index;
            _previewDamage = practice.StaminaReduction;
            
            if (healthBar != null)
            {
                healthBar.PrevieMinusHealth(_previewDamage);
            }

            _statUIUpdater.PreviewStat(_currentUnit, practice.PracticeStatType, practice.statIncrease);
            practiceNameText.gameObject.SetActive(true);
            practiceNameText.SetText($"{practice.PracticeStatName}");
            ShowArrow(index);
            ShowProbability();
        }

        #endregion

        #region UI Helpers

        private void ShowArrow(int index)
        {
            if (arrowObjs == null) return;

            for (int i = 0; i < arrowObjs.Count; i++)
            {
                if (arrowObjs[i] != null)
                {
                    arrowObjs[i].gameObject.SetActive(i == index);
                }
            }
        }

        private void HideAllArrows()
        {
            if (arrowObjs == null) return;

            foreach (var arrow in arrowObjs)
            {
                if (arrow != null)
                {
                    arrow.gameObject.SetActive(false);
                }
            }
        }

        private void ShowProbability()
        {
            if (probabilityText != null)
            {
                probabilityText.gameObject.SetActive(true);
                probabilityText.SetText($"성공확률 {Mathf.FloorToInt(_currentCondition)}%");
            }
        }

        private void HideProbabilityText()
        {
            if (probabilityText != null)
            {
                probabilityText.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}