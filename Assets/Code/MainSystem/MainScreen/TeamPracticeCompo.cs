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
        [SerializeField] private TextMeshProUGUI probabilityText;
        [SerializeField] private Button backButton;
        [SerializeField] private GameObject teamPanel;

        [Header("Member Buttons")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Button Move")]
        [SerializeField] private float liftY = 20f;

        [Header("Health Bars")]
        [SerializeField] private HealthBar teamHealthBar;
        
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
            backButton.onClick.AddListener(OnClickBack);
            
            LoadTeamPracticeData();
        }

        private void Start()
        {
            if (backButton != null)
            {
                backButton.gameObject.SetActive(false);
            }
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
                    Debug.Log("Team practice data loaded successfully");
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
            if (_unitMap == null || !_unitMap.ContainsKey(member))
            {
                Debug.LogWarning($"Unit map not initialized or member {member} not found");
                return;
            }
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
            if (_selectedMembers.Count < 2)
            {
                Debug.LogWarning("Need at least 2 members selected");
                return;
            }
            
            if (_teamPracticeData == null)
            {
                Debug.LogError("Cannot start practice: Team practice data not loaded!");
                return;
            }
            
            Debug.Log($"Starting practice with {_selectedMembers.Count} members");
            Bus<TeamPracticeEvent>.Raise(new TeamPracticeEvent(_selectedMembers.Select(t => t.currentCondition).ToList()));
        }

        private void BuildResultCache()
        {
            Debug.Log("=== BuildResultCache START ===");
            
            if (_selectedMembers == null)
            {
                Debug.LogError("_selectedMembers is null!");
                return;
            }
            
            if (_selectedMembers.Count == 0)
            {
                Debug.LogError("_selectedMembers is empty!");
                return;
            }
            
            Debug.Log($"Selected members count: {_selectedMembers.Count}");
            
            if (_teamPracticeData == null)
            {
                Debug.LogError("Team practice data is not loaded!");
                return;
            }
            
            Debug.Log("Team practice data is loaded");
            
            float totalConditionBefore = _selectedMembers.Sum(u => u.currentCondition);
            float avgConditionAfter = (totalConditionBefore / _selectedMembers.Count) - teamConditionCost;

            TeamPracticeResultCache.IsSuccess = _wasSuccess;
            TeamPracticeResultCache.SelectedMembers = new List<UnitDataSO>(_selectedMembers);
            TeamPracticeResultCache.StatDeltaDict = new Dictionary<(MemberType, StatType), int>();

            float totalTeamStatDelta = 0;

            if (_wasSuccess)
            {
                Debug.Log("Practice was successful, calculating stat gains");
                
                var statManager = StatManager.Instance;
                if (statManager == null)
                {
                    Debug.LogError("StatManager.Instance is null!");
                    return;
                }
                
                var traitManager = TraitManager.Instance;
                if (traitManager == null)
                {
                    Debug.LogError("TraitManager.Instance is null!");
                    return;
                }
                
                var ensembleModule = statManager.GetEnsembleModuleHandler();
                if (ensembleModule == null)
                {
                    Debug.LogError("Ensemble module is null!");
                    return;
                }
                
                bool isMentalPractice = _teamPracticeData.PracticeStatType == StatType.Mental;
                bool hasEntertainerBonus = isMentalPractice && _selectedMembers.Any(u => traitManager.HasTrait(u.memberType, TraitType.Entertainer));

                foreach (var unit in _selectedMembers)
                {
                    if (unit == null)
                    {
                        Debug.LogError("Unit in _selectedMembers is null!");
                        continue;
                    }
                    
                    var memberType = unit.memberType;
                    var statType = _teamPracticeData.PracticeStatType;
                    
                    float finalStatGain = ensembleModule.ApplyEnsembleBonus(teamStatIncrease, memberType);
                    
                    if (hasEntertainerBonus)
                    {
                        var holder = traitManager.GetHolder(memberType);
                        if (holder != null)
                        {
                            finalStatGain = holder.GetCalculatedStat(TraitTarget.Mental, finalStatGain);
                        }
                    }

                    int roundedStatGain = Mathf.RoundToInt(finalStatGain);
                    
                    var memberStat = statManager.GetTeamStat(statType);
                    if (memberStat == null)
                    {
                        Debug.LogError($"GetMemberStat returned null for {memberType}, {statType}");
                        continue;
                    }
                    
                    memberStat.PlusValue(roundedStatGain);
                    TeamPracticeResultCache.StatDeltaDict[(memberType, statType)] = roundedStatGain;
    
                    totalTeamStatDelta += finalStatGain;
                }
            }
            
            UpdateMembersCondition();
            
            Debug.Log("Getting first unit for team stat");
            var firstUnit = _selectedMembers[0];
            if (firstUnit == null)
            {
                Debug.LogError("firstUnit is null!");
                return;
            }
            
            Debug.Log($"First unit: {firstUnit.memberType}");
            
            if (firstUnit.teamStat == null)
            {
                Debug.LogError("firstUnit.teamStat is null!");
            }
            else
            {
                TeamPracticeResultCache.TeamStat = firstUnit.teamStat;
            }
            
            TeamPracticeResultCache.TeamStatDelta = Mathf.RoundToInt(totalTeamStatDelta);

            if (TeamPracticeResultCache.TeamStatDelta > 0)
            {
                var statManager = StatManager.Instance;
                if (statManager != null)
                {
                    var teamHarmonyStat = statManager.GetTeamStat(StatType.TeamHarmony);
                    if (teamHarmonyStat != null)
                    {
                        teamHarmonyStat.PlusValue(TeamPracticeResultCache.TeamStatDelta);
                    }
                    else
                    {
                        Debug.LogError("GetTeamStat(TeamHarmony) returned null");
                    }
                }
            }

            TeamPracticeResultCache.TeamConditionCurrent = avgConditionAfter;
            TeamPracticeResultCache.TeamConditionDelta = -teamConditionCost;
            
            Debug.Log("=== BuildResultCache END ===");
        }

        private void UpdateMembersCondition()
        {
            if (_selectedMembers == null) return;
            
            foreach (var unit in _selectedMembers)
            {
                if (unit == null) continue;
                
                unit.currentCondition = Mathf.Clamp(unit.currentCondition - teamConditionCost, 0, unit.maxCondition);

                if (TrainingManager.Instance != null)
                {
                    TrainingManager.Instance.MarkMemberTrained(unit.memberType);
                }
            }
        }

        public void OnClickBack()
        {
            _isTeamPracticeMode = false;
            ResetSelection();
            teamPanel.gameObject.SetActive(false);
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
            backButton.gameObject.SetActive(_isTeamPracticeMode);
            
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