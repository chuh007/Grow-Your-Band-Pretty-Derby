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
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Manager;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        [SerializeField] private HealthBar teamHealthBar;

        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI probabilityText;
        
        [Header("Team Practice Data")]
        [SerializeField] private AssetReference teamPracticeDataReference;

        private readonly Dictionary<MemberType, Button> _buttonMap = new();
        private readonly Dictionary<MemberType, Vector3> _originalPosMap = new();
        private readonly List<UnitDataSO> _selectedMembers = new();

        private Dictionary<MemberType, UnitDataSO> _unitMap;
        private bool _isTeamPracticeMode = false;
        private bool _wasSuccess;
        private PersonalpracticeDataSO _teamPracticeData;

        private float teamConditionCost => _teamPracticeData != null ? _teamPracticeData.StaminaReduction : 10f;
        private float teamStatIncrease => _teamPracticeData != null ? _teamPracticeData.statIncrease : 1f;

        private void Awake()
        {
            Bus<TeamPracticeResultEvent>.OnEvent += OnPracticeResult;
            enterTeamPracticeButton.onClick.AddListener(OnEnterTeamPractice);
            startPracticeButton.onClick.AddListener(OnClickStartPractice);
            
            LoadTeamPracticeData();
        }

        private void OnDestroy()
        {
            Bus<TeamPracticeResultEvent>.OnEvent -= OnPracticeResult;
            
            if (teamPracticeDataReference != null && teamPracticeDataReference.IsValid())
            {
                teamPracticeDataReference.ReleaseAsset();
            }
        }

        private void LoadTeamPracticeData()
        {
            if (teamPracticeDataReference == null) return;

            teamPracticeDataReference.LoadAssetAsync<PersonalpracticeDataSO>().Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _teamPracticeData = handle.Result;
                }
                else
                {
                    Debug.LogError("Failed to load team practice data");
                }
            };
        }

        private void OnPracticeResult(TeamPracticeResultEvent evt)
        {
            _wasSuccess = evt.IsSuccess;
            BuildResultCache();
            SceneManager.LoadScene("Ensembleproductiontest");
        }

        public void CacheUnits(List<UnitDataSO> units)
        {
            _unitMap = new Dictionary<MemberType, UnitDataSO>();
            foreach (var unit in units)
                _unitMap[unit.memberType] = unit;

            InitButtons();
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

            teamHealthBar.SetHealth(unit.currentCondition, unit.maxCondition);
            teamHealthBar.PrevieMinusHealth(teamConditionCost);
        }

        private void DeactivateMember(MemberType member)
        {
            var unit = _unitMap[member];
            _selectedMembers.Remove(unit);

            _buttonMap[member].transform.localPosition = _originalPosMap[member];

            teamHealthBar.SetHealth(unit.currentCondition, unit.maxCondition);
        }

        private void OnClickStartPractice()
        {
            if (_selectedMembers.Count < 2) return;
            
            Bus<TeamPracticeEvent>.Raise(new TeamPracticeEvent(_selectedMembers.Select(t => t.currentCondition).ToList()));
        }

        private void BuildResultCache()
        {
            if (_selectedMembers == null || _selectedMembers.Count == 0) return;
            
            float totalConditionBefore = _selectedMembers.Sum(u => u.currentCondition);
            float avgConditionAfter = (totalConditionBefore / _selectedMembers.Count) - teamConditionCost;

            TeamPracticeResultCache.IsSuccess = _wasSuccess;
            TeamPracticeResultCache.SelectedMembers = new List<UnitDataSO>(_selectedMembers);
            TeamPracticeResultCache.StatDeltaDict = new Dictionary<(MemberType, StatType), int>();

            float totalTeamStatDelta = 0;

            if (_wasSuccess)
            {
                var statManager = StatManager.Instance;
                var traitManager = TraitManager.Instance;
                var ensembleModule = statManager.GetEnsembleModuleHandler();
                
                bool isMentalPractice = _teamPracticeData.PracticeStatType == StatType.Mental;
                bool hasEntertainerBonus = isMentalPractice && _selectedMembers.Any(u => traitManager.HasTrait(u.memberType, TraitType.Entertainer));

                foreach (var unit in _selectedMembers)
                {
                    var memberType = unit.memberType;
                    var statType = _teamPracticeData.PracticeStatType;
                    
                    float finalStatGain = ensembleModule.ApplyEnsembleBonus(teamStatIncrease, memberType);
                    
                    if (hasEntertainerBonus)
                    {
                        var holder = traitManager.GetHolder(memberType);
                        finalStatGain = holder.GetCalculatedStat(TraitTarget.Mental, finalStatGain);
                    }

                    int roundedStatGain = Mathf.RoundToInt(finalStatGain);
                    statManager.GetMemberStat(memberType, statType).PlusValue(roundedStatGain);
                    TeamPracticeResultCache.StatDeltaDict[(memberType, statType)] = roundedStatGain;
    
                    totalTeamStatDelta += finalStatGain;
                }
            }
            
            UpdateMembersCondition();
            
            var firstUnit = _selectedMembers[0];
            TeamPracticeResultCache.TeamStat = firstUnit.teamStat;
            TeamPracticeResultCache.TeamStatDelta = Mathf.RoundToInt(totalTeamStatDelta);

            if (TeamPracticeResultCache.TeamStatDelta > 0)
            {
                StatManager.Instance.GetTeamStat(StatType.TeamHarmony)
                    .PlusValue(TeamPracticeResultCache.TeamStatDelta);
            }

            TeamPracticeResultCache.TeamConditionCurrent = avgConditionAfter;
            TeamPracticeResultCache.TeamConditionDelta = -teamConditionCost;
        }

        private void UpdateMembersCondition()
        {
            foreach (var unit in _selectedMembers)
            {
                unit.currentCondition = Mathf.Clamp(unit.currentCondition - teamConditionCost, 0, unit.maxCondition);

                TrainingManager.Instance.MarkMemberTrained(unit.memberType);
            }
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
            }

            teamHealthBar.PrevieMinusHealth(0);
            _selectedMembers.Clear();
            UpdateUI();
        }

        private void UpdateUI()
        {
            startPracticeButton.interactable = _isTeamPracticeMode && _selectedMembers.Count >= 2;
            if (_selectedMembers.Count >= 2)
            {
                var conditions = _selectedMembers.Select(u => u.currentCondition).ToList();
                float successRate = StatManager.Instance.GetEnsembleSuccessRate(conditions);
                probabilityText.SetText($"성공확률 : {successRate}%"); 
            }
            else
            {
                probabilityText.SetText("0%");
            }
        }
    }
}