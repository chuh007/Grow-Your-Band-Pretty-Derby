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
        [Header("UI")]
        [SerializeField] private Button enterTeamPracticeButton;
        [SerializeField] private Button startPracticeButton;

        [Header("Member Buttons")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Button Move")]
        [SerializeField] private float liftY = 20f;
        
        [FormerlySerializedAs("trainingSequenceController")]
        [Header("TrainingSequence")]
        [SerializeField] private TeamTrainingSequenceController personalTrainingSequenceController;

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
        }

        private void DeactivateMember(MemberType member)
        {
            _selectedMembers.Remove(_unitMap[member]);

            var tr = _buttonMap[member].transform;
            tr.localPosition = _originLocalPosMap[member];
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
            }

            _selectedMembers.Clear();
            UpdateUI();
        }


        #endregion
    }
}
