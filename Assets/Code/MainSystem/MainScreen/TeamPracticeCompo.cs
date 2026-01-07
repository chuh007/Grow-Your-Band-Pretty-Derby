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

namespace Code.MainSystem.MainScreen
{
    public class TeamPracticeCompo : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button enterTeamPracticeButton;
        [SerializeField] private Button startPracticeButton;
        [SerializeField] private Button backButton;

        [Header("Member Buttons")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Button Move")]
        [SerializeField] private float liftY = 20f;

        private readonly Dictionary<MemberType, Button> _buttonMap = new();
        private readonly Dictionary<MemberType, Vector3> _originLocalPosMap = new();
        private readonly HashSet<MemberType> _selectedMembers = new();

        private Dictionary<MemberType, UnitDataSO> _unitMap;
        private bool _isTeamPracticeMode = false;

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
            //evt.IsSuccess;
            //이거 불 값으로 true, false보내고 있음
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
            backButton.onClick.AddListener(OnClickBack);
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

            if (_selectedMembers.Contains(member))
                DeactivateMember(member);
            else
                ActivateMember(member);

            UpdateUI();
        }

        private void ActivateMember(MemberType member)
        {
            _selectedMembers.Add(member);

            var tr = _buttonMap[member].transform;
            tr.localPosition = _originLocalPosMap[member] + Vector3.up * liftY;
        }

        private void DeactivateMember(MemberType member)
        {
            _selectedMembers.Remove(member);

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

        private void OnClickStartPractice()
        {
            if (!_isTeamPracticeMode) return;
            if (_selectedMembers.Count < 2) return;
            
            List<float> memberConditions = new List<float>();
            foreach (var member in _selectedMembers)
                if (_unitMap.TryGetValue(member, out var unit))
                    memberConditions.Add(unit.currentCondition);

            Bus<TeamPracticeEvent>.Raise(new TeamPracticeEvent(memberConditions));
            TrainingManager.Instance.MarkMembersTrainedForTeam(_selectedMembers);
        }

        private void OnClickBack()
        {
            _isTeamPracticeMode = false;
            ResetSelection();
        }

        private void ResetSelection()
        {
            foreach (var member in _selectedMembers)
            {
                if (_buttonMap.TryGetValue(member, out var btn))
                {
                    btn.transform.localPosition = _originLocalPosMap[member];
                }
            }

            _selectedMembers.Clear();
            UpdateUI();
        }

        #endregion
    }
}
