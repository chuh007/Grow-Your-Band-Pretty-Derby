using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Etc;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.StatSystem.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.MainScreen
{
    public class MainScreen : MonoBehaviour
    {
        [Header("Addressables Keys/Labels")]
        [SerializeField] private string unitLabel = "Units";

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI charterNameText;
        [SerializeField] private List<TextMeshProUGUI> statNameTexts;
        [SerializeField] private List<TextMeshProUGUI> statValueTexts;
        [SerializeField] private List<Image> statIcons;
        [SerializeField] private TextMeshProUGUI conditionText;
        [SerializeField] private Image characterIcon;
        [SerializeField] private GameObject teamPanel;

        [Header("Member Carousel (새 UI 시스템)")]
        [SerializeField] private MemberCarousel memberCarousel;

        [Header("Components")]
        [SerializeField] private PersonalPracticeCompo personalPracticeCompo;
        [SerializeField] private TeamPracticeCompo teamPracticeCompo;
        [SerializeField] private RestSelectCompo restSelectCompo;

        public UnitSelector UnitSelector { get; private set; }

        private StatUIUpdater _statUIUpdater;
        private List<UnitDataSO> _loadedUnits;
        
        private static bool _returnedFromTeamPractice = false;

        #region Unity LifeCycle

        private async void Start()
        {
            await LoadUnitsAsync();
            
            if (_returnedFromTeamPractice)
            {
                await CheckTeamPracticeReturn();
            }
        }

        private void OnEnable()
        {
            Bus<MemberTrainingStateChangedEvent>.OnEvent += OnMemberTrainingStateChanged;
        }

        private void OnDisable()
        {
            Bus<MemberTrainingStateChangedEvent>.OnEvent -= OnMemberTrainingStateChanged;
        }

        #endregion

        #region Init

        private async Task LoadUnitsAsync()
        {
            _loadedUnits = await GameManager.Instance.LoadAllAddressablesAsync<UnitDataSO>(unitLabel);

            UnitSelector = new UnitSelector();
            UnitSelector.Init(_loadedUnits);
            
            while (StatManager.Instance != null && !StatManager.Instance.IsInitialized)
            {
                await Task.Yield();
            }

            _statUIUpdater = new StatUIUpdater(statNameTexts, statValueTexts, statIcons, StatManager.Instance);
            
            if (memberCarousel != null)
            {
                memberCarousel.Init(_loadedUnits, OnMemberSelectedFromCarousel);
            }
            else
            {
                Debug.LogWarning("MemberCarousel이 할당되지 않았습니다!");
                if (_loadedUnits.Count > 0)
                {
                    SelectUnit(_loadedUnits[0]);
                }
            }
            
            teamPracticeCompo.CacheUnits(_loadedUnits);
        }
        
        private async UniTask CheckTeamPracticeReturn()
        {
            await UniTask.Delay(100);
            
            if (CommentManager.instance != null)
            {
                await CommentManager.instance.ShowAllComments();
            }
            
            await UniTask.Yield();

            _returnedFromTeamPractice = false;
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
        }

        #endregion

        #region Event Handling

        private void OnMemberTrainingStateChanged(MemberTrainingStateChangedEvent evt)
        {
        }

        /// <summary>
        /// 캐러셀에서 멤버가 선택되었을 때 호출되는 콜백
        /// </summary>
        private void OnMemberSelectedFromCarousel(UnitDataSO unit)
        {
            if (unit != null)
            {
                SelectUnit(unit);
            }
        }

        #endregion

        #region UI Interaction

        public void TeamButtonClicked()
        {
            teamPanel.gameObject.SetActive(true);
        }

        #endregion

        #region Unit Selection

        private void SelectUnit(UnitDataSO unit)
        {
            if (unit == null) return;

            personalPracticeCompo.Init(unit, _statUIUpdater);

            charterNameText.SetText(unit.unitName);
            conditionText.SetText($"{unit.currentCondition}/{unit.maxCondition}");

            _statUIUpdater.UpdateAll(unit);

            LoadUnitSprite(unit);
            
            if (UnitSelector != null)
            {
                UnitSelector.SetCurrentUnit(unit);
            }
        }

        private async void LoadUnitSprite(UnitDataSO unit)
        {
            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
                return;
            
            restSelectCompo.CacheUnits(_loadedUnits);
            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
            
            if (characterIcon != null && sprite != null)
            {
                characterIcon.sprite = sprite;
                characterIcon.color = Color.white;
            }
        }

        #endregion
        
        public void SetReturnedFromTeamPractice()
        {
            _returnedFromTeamPractice = true;
        }

        /// <summary>
        /// 외부에서 특정 멤버를 선택하도록 요청할 때 사용
        /// </summary>
        public void SelectMemberByType(MemberType memberType)
        {
            if (memberCarousel != null)
            {
                memberCarousel.SelectMember(memberType);
            }
            else
            {
                var unit = _loadedUnits.FirstOrDefault(u => u.memberType == memberType);
                if (unit != null)
                {
                    SelectUnit(unit);
                }
            }
        }
    }
}