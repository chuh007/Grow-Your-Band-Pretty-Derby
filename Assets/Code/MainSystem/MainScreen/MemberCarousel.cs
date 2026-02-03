using System.Collections.Generic;
using Code.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen
{
    /// <summary>
    /// 멤버 캐러셀 UI 컨트롤러 (고정된 5개 아이콘, 테두리만 이동)
    /// </summary>
    public class MemberCarousel : MonoBehaviour
    {
        [Header("Member Icon Slots")]
        [SerializeField] private List<Image> memberIconSlots;
        
        [Header("Selection Border")]
        [SerializeField] private RectTransform selectionBorder;
        
        [Header("Arrow Buttons")]
        [SerializeField] private Button leftArrowButton;
        [SerializeField] private Button rightArrowButton;
        
        [Header("Visual Settings")]
        [SerializeField] private float borderMoveDuration = 0.3f;
        [SerializeField] private Ease borderMoveEase = Ease.OutCubic;
        
        private List<UnitDataSO> _allUnits;
        private int _currentSelectedIndex = 2;
        private bool _isTransitioning = false;
        
        private System.Action<UnitDataSO> _onMemberSelected;

        public void Init(List<UnitDataSO> units, System.Action<UnitDataSO> onMemberSelected)
        {
            _allUnits = units;
            _onMemberSelected = onMemberSelected;
            
            if (_allUnits == null || _allUnits.Count == 0)
            {
                Debug.LogError("Units list is empty!");
                return;
            }
            
            if (memberIconSlots == null || memberIconSlots.Count != 5)
            {
                Debug.LogError($"Member Icon Slots must be exactly 5!");
                return;
            }
            
            if (leftArrowButton != null)
                leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
            else
                Debug.LogError("Left Arrow Button is NULL!");
                
            if (rightArrowButton != null)
                rightArrowButton.onClick.AddListener(OnRightArrowClicked);
            else
                Debug.LogError("Right Arrow Button is NULL!");
            
            if (selectionBorder == null)
            {
                Debug.LogError("Selection Border is NULL!");
                return;
            }
            
            LoadAllIcons();
            
            MoveBorderToIndex(_currentSelectedIndex, false);
            
            if (_allUnits.Count > _currentSelectedIndex)
            {
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            }
        }

        private void OnDestroy()
        {
            if (leftArrowButton != null)
                leftArrowButton.onClick.RemoveListener(OnLeftArrowClicked);
            if (rightArrowButton != null)
                rightArrowButton.onClick.RemoveListener(OnRightArrowClicked);
        }

        /// <summary>
        /// 5개 아이콘에 각 유닛의 스프라이트를 로드
        /// </summary>
        private async void LoadAllIcons()
        {
            int count = Mathf.Min(_allUnits.Count, memberIconSlots.Count);
            
            for (int i = 0; i < count; i++)
            {
                var unit = _allUnits[i];
                var slot = memberIconSlots[i];
                
                if (slot == null || unit == null || string.IsNullOrEmpty(unit.spriteAddressableKey))
                    continue;
                
                try
                {
                    var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
                    
                    if (slot != null && sprite != null)
                    {
                        slot.sprite = sprite;
                        slot.color = Color.white;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load icon for {unit.unitName}: {e.Message}");
                }
            }
        }

        private void OnLeftArrowClicked()
        {
            if (_isTransitioning) return;
            
            MoveSelectionLeft();
        }

        private void OnRightArrowClicked()
        {
            if (_isTransitioning) return;
            
            MoveSelectionRight();
        }

        /// <summary>
        /// 선택을 왼쪽으로 이동
        /// </summary>
        private void MoveSelectionLeft()
        {
            _isTransitioning = true;
            
            _currentSelectedIndex--;
            if (_currentSelectedIndex < 0)
                _currentSelectedIndex = _allUnits.Count - 1;
            
            MoveBorderToIndex(_currentSelectedIndex, true, () =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            });
        }

        /// <summary>
        /// 선택을 오른쪽으로 이동
        /// </summary>
        private void MoveSelectionRight()
        {
            _isTransitioning = true;
            
            _currentSelectedIndex++;
            if (_currentSelectedIndex >= _allUnits.Count)
                _currentSelectedIndex = 0;
            
            MoveBorderToIndex(_currentSelectedIndex, true, () =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            });
        }

        /// <summary>
        /// 테두리를 특정 인덱스의 슬롯 위치로 이동
        /// </summary>
        private void MoveBorderToIndex(int index, bool animate, System.Action onComplete = null)
        {
            if (index < 0 || index >= memberIconSlots.Count)
            {
                onComplete?.Invoke();
                return;
            }
    
            if (selectionBorder == null)
            {
                onComplete?.Invoke();
                return;
            }
    
            RectTransform targetSlot = memberIconSlots[index].rectTransform;
            
            selectionBorder.SetParent(targetSlot, false);
    
            if (animate)
            {
                selectionBorder.DOAnchorPos(Vector2.zero, borderMoveDuration)
                    .SetEase(borderMoveEase)
                    .OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                selectionBorder.anchoredPosition = Vector2.zero;
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 특정 멤버를 직접 선택
        /// </summary>
        public void SelectMember(MemberType memberType)
        {
            if (_allUnits == null || _allUnits.Count == 0)
                return;
                
            int index = _allUnits.FindIndex(u => u.memberType == memberType);
            if (index >= 0 && index != _currentSelectedIndex)
            {
                _currentSelectedIndex = index;
                MoveBorderToIndex(_currentSelectedIndex, true, () =>
                {
                    _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
                });
            }
        }

        /// <summary>
        /// 슬롯을 직접 클릭했을 때
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            if (_isTransitioning) return;
            if (slotIndex < 0 || slotIndex >= memberIconSlots.Count) return;
            if (slotIndex >= _allUnits.Count) return;
            
            if (slotIndex != _currentSelectedIndex)
            {
                _currentSelectedIndex = slotIndex;
                MoveBorderToIndex(_currentSelectedIndex, true, () =>
                {
                    _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
                });
            }
        }

        /// <summary>
        /// 현재 선택된 멤버 가져오기
        /// </summary>
        public UnitDataSO GetCurrentMember()
        {
            if (_allUnits == null || _allUnits.Count == 0) return null;
            if (_currentSelectedIndex < 0 || _currentSelectedIndex >= _allUnits.Count) return null;
            return _allUnits[_currentSelectedIndex];
        }
    }
}