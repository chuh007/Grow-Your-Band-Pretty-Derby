using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;

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

        [Header("Health Bars")]
        [SerializeField] private List<UnitHealthBars> unitHealthBars;
        [SerializeField] private HealthBar teamHealthBar;

        [Header("Practice Settings")]
        [SerializeField] private float teamConditionCost = 5f;
        [SerializeField] private MainScreen mainScreen;

        private readonly Dictionary<MemberType, Button> _buttonMap = new();
        private readonly Dictionary<MemberType, Vector3> _originalPosMap = new();
        private readonly List<UnitDataSO> _selectedMembers = new();

        private Dictionary<MemberType, UnitDataSO> _unitMap;
        private bool _isTeamPracticeMode = false;
        private bool _wasSuccess;

        private void Awake()
        {
            Bus<TeamPracticeResultEvent>.OnEvent += OnPracticeResult;
            enterTeamPracticeButton.onClick.AddListener(OnEnterTeamPractice);
            startPracticeButton.onClick.AddListener(OnClickStartPractice);
        }

        private void OnDestroy()
        {
            Bus<TeamPracticeResultEvent>.OnEvent -= OnPracticeResult;
        }

        private void OnPracticeResult(TeamPracticeResultEvent evt)
        {
            _wasSuccess = evt.IsSuccess;
            BuildResultCache();
            mainScreen.SetReturnedFromTeamPractice();
            SceneManager.LoadScene("Ensembleproductiontest");
        }

        public void CacheUnits(List<UnitDataSO> units)
        {
            _unitMap = new Dictionary<MemberType, UnitDataSO>();
            foreach (var unit in units)
                _unitMap[unit.memberType] = unit;

            InitButtons();
            InitHealthBars();
        }

        private void InitButtons()
        {
            foreach (var btn in memberButtons)
            {
                if (Enum.TryParse(btn.name, out MemberType type))
                {
                    _buttonMap[type] = btn;
                    _originalPosMap[type] = btn.transform.localPosition;
                }
            }
        }

        private void InitHealthBars()
        {
            foreach (var bar in unitHealthBars)
            {
                if (_unitMap.TryGetValue(bar.memberType, out var unit))
                    bar.healthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            }
        }

        private void OnEnterTeamPractice()
        {
            _isTeamPracticeMode = true;
            ResetSelection();
            UpdateUI();
        }

        public void OnMemberButtonClicked(string name)
        {
            if (!Enum.TryParse(name, out MemberType member)) return;
            if (!_isTeamPracticeMode || TrainingManager.Instance.IsMemberTrained(member)) return;

            var unit = _unitMap[member];
            if (_selectedMembers.Contains(unit))
                DeactivateMember(member);
            else
                ActivateMember(member);

            UpdateUI();
        }

        private void ActivateMember(MemberType member)
        {
            var unit = _unitMap[member];
            _selectedMembers.Add(unit);

            _buttonMap[member].transform.localPosition = _originalPosMap[member] + Vector3.up * liftY;

            var bar = unitHealthBars.Find(b => b.memberType == member);
            if (bar != null)
                bar.healthBar.PrevieMinusHealth(teamConditionCost);

            teamHealthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            teamHealthBar.PrevieMinusHealth(teamConditionCost);
        }

        private void DeactivateMember(MemberType member)
        {
            var unit = _unitMap[member];
            _selectedMembers.Remove(unit);

            _buttonMap[member].transform.localPosition = _originalPosMap[member];

            var bar = unitHealthBars.Find(b => b.memberType == member);
            if (bar != null)
                bar.healthBar.SetHealth(unit.currentCondition, unit.maxCondition);

            teamHealthBar.SetHealth(unit.currentCondition, unit.maxCondition);
        }

        private void OnClickStartPractice()
        {
            if (_selectedMembers.Count < 2) return;
            
            Bus<TeamPracticeEvent>.Raise(new TeamPracticeEvent(_selectedMembers.Select(t => t.currentCondition).ToList()));
        }

        private void BuildResultCache()
        {
            float totalConditionBefore = 0f;
            
            foreach (var unit in _selectedMembers)
            {
                totalConditionBefore += unit.currentCondition;
                unit.currentCondition = Mathf.Clamp(unit.currentCondition - teamConditionCost, 0, unit.maxCondition);

                var bar = unitHealthBars.Find(b => b.memberType == unit.memberType);
                if (bar != null)
                    bar.healthBar.ApplyHealth(teamConditionCost);

                TrainingManager.Instance.MarkMemberTrained(unit.memberType);
            }

            float avgConditionBefore = totalConditionBefore / _selectedMembers.Count;
            float avgConditionAfter = avgConditionBefore - teamConditionCost;
            
            TeamPracticeResultCache.IsSuccess = _wasSuccess;
            TeamPracticeResultCache.SelectedMembers = new List<UnitDataSO>(_selectedMembers);
            TeamPracticeResultCache.StatDeltaDict = new Dictionary<(MemberType, StatType), int>();

            var firstUnit = _selectedMembers[0];
            TeamPracticeResultCache.TeamStat = firstUnit.TeamStat;
            
            float totalTeamStatDelta = 0;
            if (_wasSuccess)
            {
                float baseGain = UnityEngine.Random.Range(1.0f, 3.0f);

                foreach (var unit in _selectedMembers)
                {
                    var ensembleModule = StatManager.Instance.GetEnsembleModuleHandler();
                    totalTeamStatDelta += ensembleModule.ApplyEnsembleBonus(baseGain, unit.memberType);
                }
            }
            
            TeamPracticeResultCache.TeamStatDelta = Mathf.RoundToInt(totalTeamStatDelta);
            
            if (TeamPracticeResultCache.TeamStatDelta > 0)
            {
                StatManager.Instance.GetTeamStat(StatType.TeamHarmony)
                    .PlusValue(TeamPracticeResultCache.TeamStatDelta);
            }

            TeamPracticeResultCache.TeamConditionCurrent = avgConditionAfter;
            TeamPracticeResultCache.TeamConditionDelta = -teamConditionCost;
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
                    btn.transform.localPosition = _originalPosMap[member.memberType];

                var bar = unitHealthBars.Find(b => b.memberType == member.memberType);
                if (bar != null)
                    bar.healthBar.SetHealth(member.currentCondition, member.maxCondition);
            }

            teamHealthBar.PrevieMinusHealth(0);
            _selectedMembers.Clear();
            UpdateUI();
        }

        private void UpdateUI()
        {
            startPracticeButton.interactable = _isTeamPracticeMode && _selectedMembers.Count >= 2;
        }
    }
}
