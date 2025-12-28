using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.MainScreen.MemberData;
using Code.Core;

namespace Code.MainSystem.MainScreen
{
    public class TeamPracticeCompo : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI successRateText;
        [SerializeField] private Button startPracticeButton;
        [SerializeField] private string RhythmGameScene;

        [Header("Member Buttons (이미 배치된 버튼들)")]
        [SerializeField] private List<Button> memberButtons;

        [Header("Button Move")]
        [SerializeField] private float liftY = 20f;

        private readonly Dictionary<MemberType, Button> _buttonMap = new();
        private readonly Dictionary<MemberType, Vector2> _originPosMap = new();
        private readonly HashSet<MemberType> _selectedMembers = new();

        private Dictionary<MemberType, UnitDataSO> _unitMap;

        #region LifeCycle

        private void Awake()
        {
           
            UpdateUI();
        }

        #endregion

        #region Init

        public void CacheUnits(List<UnitDataSO> unitData)
        {
            _unitMap = new Dictionary<MemberType, UnitDataSO>();

            var units = unitData;
            foreach (var unit in units)
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
                {
                    Debug.LogError($"버튼 이름이 MemberType과 다름 : {btn.name}");
                    continue;
                }

                _buttonMap[type] = btn;
                _originPosMap[type] = btn.GetComponent<RectTransform>().anchoredPosition;

                if (TrainingManager.Instance.IsMemberTrained(type))
                {
                    btn.interactable = false;
                    continue;
                }

                btn.onClick.AddListener(() => OnMemberButtonClicked(type));
            }

            startPracticeButton.onClick.AddListener(OnClickStartPractice);
        }

        #endregion

        #region Button Logic

        private void OnMemberButtonClicked(MemberType member)
        {
            if (_selectedMembers.Contains(member))
            {
                DeactivateMember(member);
            }
            else
            {
                ActivateMember(member);
            }

            UpdateUI();
        }

        private void ActivateMember(MemberType member)
        {
            if (TrainingManager.Instance.IsMemberTrained(member)) return;

            _selectedMembers.Add(member);

            var rt = _buttonMap[member].GetComponent<RectTransform>();
            rt.anchoredPosition = _originPosMap[member] + Vector2.up * liftY;
        }

        private void DeactivateMember(MemberType member)
        {
            _selectedMembers.Remove(member);

            var rt = _buttonMap[member].GetComponent<RectTransform>();
            rt.anchoredPosition = _originPosMap[member];
        }

        #endregion

        #region UI

        private void UpdateUI()
        {
            startPracticeButton.interactable = _selectedMembers.Count >= 2;
            UpdateSuccessRate();
        }

        private void UpdateSuccessRate()
        {
            if (_selectedMembers.Count < 2)
            {
                successRateText.gameObject.SetActive(false);
                return;
            }

            float totalRate = 0f;
            int count = 0;

            foreach (var member in _selectedMembers)
            {
                if (!_unitMap.TryGetValue(member, out var unit)) continue;
                if (unit.maxCondition <= 0) continue;

                totalRate += unit.currentCondition / unit.maxCondition;
                count++;
            }

            if (count == 0)
            {
                successRateText.gameObject.SetActive(false);
                return;
            }

            int percent = Mathf.RoundToInt((totalRate / count) * 100f);
            successRateText.gameObject.SetActive(true);
            successRateText.SetText($"성공률 {percent}%");
        }

        #endregion

        #region Start Practice

        private void OnClickStartPractice()
        {
            if (_selectedMembers.Count < 2) return;

            foreach (var member in _selectedMembers)
            {
                TrainingManager.Instance.MarkMemberTrained(member);
            }

            SceneManager.LoadScene(RhythmGameScene);
        }

        #endregion
    }
}
