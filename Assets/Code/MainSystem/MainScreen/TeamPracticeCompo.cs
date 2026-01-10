using System;
using System.Collections.Generic;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Events;
using UnityEngine.Serialization;

namespace Code.MainSystem.MainScreen
{
    public class TeamPracticeCompo : MonoBehaviour
    {
        [Serializable]
        public class UnitHealthBars
        {
            public MemberType memberType;
            public HealthBar healthBar;
        }
        
        [Header("UI")]
        [SerializeField] private Button enterTeamPracticeButton;
        [SerializeField] private Button startPracticeButton;

        [Header("Member Buttons")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Button Move")]
        [SerializeField] private float liftY = 20f;
        
        [Header("TrainingSequence")]
        [SerializeField] private TeamTrainingSequenceController personalTrainingSequenceController;
        
        [SerializeField] private List<UnitHealthBars> unitHealthBars;
        [SerializeField] private HealthBar healthBar;

        [SerializeField] private float teamCantionDes = 5f;

        private readonly Dictionary<MemberType, Button> _buttonMap = new();
        private readonly Dictionary<MemberType, Vector3> _originLocalPosMap = new();
        private readonly List<UnitDataSO> _selectedMembers = new();

        private Dictionary<MemberType, UnitDataSO> _unitMap;
        private bool _isTeamPracticeMode = false;
        private bool _issuccess;

        #region Unity LifeCycle

        private void Awake()
        {
            UpdateUI();
            Bus<TeamPracticeResultEvent>.OnEvent += HandleTeamPracticeResult;
        }

        private void OnDestroy()
        {
            Bus<TeamPracticeResultEvent>.OnEvent -= HandleTeamPracticeResult;
        }

        #endregion
        
        private void HandleTeamPracticeResult(TeamPracticeResultEvent evt)
        {
            _issuccess = evt.IsSuccess;
        }

        #region Init

        public void CacheUnits(List<UnitDataSO> unitData)
        {
            _unitMap = new Dictionary<MemberType, UnitDataSO>();
            foreach (var unit in unitData)
            {
                _unitMap[unit.memberType] = unit;
            }

            InitButtons();
            InitHealthBars();
        }

        private void InitButtons()
        {
            foreach (var btn in memberButtons)
            {
                if (!Enum.TryParse(btn.name, out MemberType type))
                    continue;

                _buttonMap[type] = btn;
                _originLocalPosMap[type] = btn.transform.localPosition;
            }

            enterTeamPracticeButton.onClick.AddListener(OnEnterTeamPractice);
            startPracticeButton.onClick.AddListener(OnClickStartPractice);
        }
        
        private void InitHealthBars()
        {
            foreach (var unitBar in unitHealthBars)
            {
                if (_unitMap.TryGetValue(unitBar.memberType, out var unit))
                {
                    unitBar.healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
                }
            }
        }


        #endregion

        #region Team Practice Mode

        private void OnEnterTeamPractice()
        {
            _isTeamPracticeMode = true;
            ResetSelection();
            UpdateUI();
        }

        #endregion

        #region Member Button Logic
        
        public void OnMemberButtonClicked(string memberName)
        {
            if (!Enum.TryParse(memberName, out MemberType member))
                return;

            if (!_isTeamPracticeMode)
                return;

            if (TrainingManager.Instance.IsMemberTrained(member))
                return;

            if (_selectedMembers.Contains(_unitMap[member]))
                DeactivateMember(member);
            else
                ActivateMember(member);

            UpdateUI();
        }

        private void ActivateMember(MemberType member)
        {
            _selectedMembers.Add(_unitMap[member]);

            var tr = _buttonMap[member].transform;
            tr.localPosition = _originLocalPosMap[member] + Vector3.up * liftY;

            var unit = _unitMap[member];
            var unitHealth = unitHealthBars.Find(u => u.memberType == member);
            if (unitHealth != null)
            {
                unitHealth.healthBar.PrevieMinusHealth(teamCantionDes);
            }
            
            healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            healthBar.PrevieMinusHealth(teamCantionDes);
        }



        private void DeactivateMember(MemberType member)
        {
            _selectedMembers.Remove(_unitMap[member]);

            var tr = _buttonMap[member].transform;
            tr.localPosition = _originLocalPosMap[member];

            if (_unitMap.TryGetValue(member, out var unit))
            {
                var unitHealth = unitHealthBars.Find(u => u.memberType == member);
                if (unitHealth != null)
                {
                    unitHealth.healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
                }

                healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            }
        }


        #endregion

        #region UI

        private void UpdateUI()
        {
            startPracticeButton.interactable = _isTeamPracticeMode && _selectedMembers.Count >= 2;
        }

        #endregion

        #region Practice Start / Back

        private async void OnClickStartPractice()
        {
            if (!_isTeamPracticeMode) return;
            if (_selectedMembers.Count < 2) return;

            List<float> memberConditions = new List<float>();
            foreach (var member in _selectedMembers)
                memberConditions.Add(member.currentCondition);

            Bus<TeamPracticeEvent>.Raise(new TeamPracticeEvent(memberConditions));

            foreach (var member in _selectedMembers)
            {
                float staminaReduction = teamCantionDes;
                float newCondition = Mathf.Clamp(member.currentCondition - staminaReduction, 0, member.maxCondition);
                member.currentCondition = newCondition;
                
                var unitHealth = unitHealthBars.Find(u => u.memberType == member.memberType);
                if (unitHealth != null)
                {
                    unitHealth.healthBar.ApplyHealth(staminaReduction);
                }
                
                TrainingManager.Instance.MarkMemberTrained(member.memberType);
            }

            personalTrainingSequenceController.gameObject.SetActive(true);
            var teamPracticeType = new TeamTrainingType(_selectedMembers);
            await personalTrainingSequenceController.PlayTeamTrainingSequence(_issuccess, teamPracticeType, _selectedMembers);

            OnClickBack();
        }



        public void OnClickBack()
        {
            _isTeamPracticeMode = false;
            ResetSelection();
        }

        private void ResetSelection()
        {
            foreach (var member in _selectedMembers)
            {
                if (_buttonMap.TryGetValue(member.memberType, out var btn))
                {
                    btn.transform.localPosition = _originLocalPosMap[member.memberType];
                }

                var unitHealth = unitHealthBars.Find(u => u.memberType == member.memberType);
                if (unitHealth != null)
                {
                    unitHealth.healthBar.SetHealth(member.currentCondition, member.maxCondition);
                }
            }
            
            if (_selectedMembers.Count > 0)
            {
                var last = _selectedMembers[^1];
                healthBar.SetHealth(last.currentCondition, last.maxCondition);
            }
            else
            {
                healthBar.SetHealth(0f, 100f); 
            }

            _selectedMembers.Clear();
            UpdateUI();
        }



        #endregion
    }
}
