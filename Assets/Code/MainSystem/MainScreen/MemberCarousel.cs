using System.Collections.Generic;
using Code.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen
{
    /// <summary>
    /// 멤버 캐러셀 UI 컨트롤러 (고정된 5개 아이콘, 테두리만 이동)
    /// 행동한 멤버는 회색 처리되고 건너뛰어집니다
    /// </summary>
    public class MemberCarousel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        [SerializeField] private Color activeColor = Color.white;
        
        [Header("Swipe Settings")]
        [SerializeField] private float swipeThreshold = 50f;
        
        private List<UnitDataSO> _allUnits;
        private int _currentSelectedIndex = 2;
        private bool _isTransitioning = false;
        
        private System.Action<UnitDataSO> _onMemberSelected;
        
        private Vector2 _dragStartPos;
        private bool _isDragging = false;

        private Dictionary<MemberType, bool> _memberActionStates = new Dictionary<MemberType, bool>();

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
            
            InitializeActionStates();
            
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

        #region Action State Management

        /// <summary>
        /// 모든 멤버를 활성 상태로 초기화
        /// </summary>
        private void InitializeActionStates()
        {
            _memberActionStates.Clear();
            
            foreach (var unit in _allUnits)
            {
                _memberActionStates[unit.memberType] = false;
            }
        }

        /// <summary>
        /// 특정 멤버의 행동 상태 업데이트
        /// </summary>
        public void UpdateMemberActionState(MemberType memberType, bool hasActed)
        {
            _memberActionStates[memberType] = hasActed;
            UpdateIconVisuals();
            
            Debug.Log($"[MemberCarousel] {memberType} action state updated to {hasActed}");
        }

        /// <summary>
        /// 모든 멤버의 행동 상태 초기화 (턴 시작 시)
        /// </summary>
        public void ResetAllActionStates()
        {
            InitializeActionStates();
            UpdateIconVisuals();
            
            Debug.Log("[MemberCarousel] All action states reset");
        }

        /// <summary>
        /// 아이콘 색상 업데이트
        /// </summary>
        private void UpdateIconVisuals()
        {
            if (_allUnits == null || memberIconSlots == null)
            {
                Debug.LogWarning("[MemberCarousel] UpdateIconVisuals called before initialization");
                return;
            }
            
            for (int i = 0; i < memberIconSlots.Count && i < _allUnits.Count; i++)
            {
                var slot = memberIconSlots[i];
                var unit = _allUnits[i];
                
                if (slot == null || unit == null)
                    continue;
                
                bool hasActed = _memberActionStates.ContainsKey(unit.memberType) 
                    ? _memberActionStates[unit.memberType] 
                    : false;
                
                slot.color = hasActed ? inactiveColor : activeColor;
            }
        }

        /// <summary>
        /// 특정 멤버가 행동했는지 확인
        /// </summary>
        private bool HasMemberActed(int index)
        {
            if (index < 0 || index >= _allUnits.Count)
                return false;
            
            var memberType = _allUnits[index].memberType;
            return _memberActionStates.ContainsKey(memberType) ? _memberActionStates[memberType] : false;
        }

        #endregion

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

                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load icon for {unit.unitName}: {e.Message}");
                }
            }
            
            UpdateIconVisuals();
        }

        #region Swipe Handlers

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isTransitioning) return;
            
            _dragStartPos = eventData.position;
            _isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging || _isTransitioning) return;
            
            _isDragging = false;
            
            Vector2 dragEndPos = eventData.position;
            Vector2 dragDelta = dragEndPos - _dragStartPos;
            
            if (Mathf.Abs(dragDelta.x) > swipeThreshold)
            {
                if (dragDelta.x > 0)
                {
                    MoveSelectionLeft();
                }
                else
                {
                    MoveSelectionRight();
                }
            }
        }

        #endregion

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
        /// 선택을 왼쪽으로 이동 (행동한 멤버 건너뛰기)
        /// </summary>
        private void MoveSelectionLeft()
        {
            _isTransitioning = true;
            
            int nextIndex = FindNextActiveIndex(_currentSelectedIndex, -1);
            
            if (nextIndex == -1)
            {
                Debug.LogWarning("[MemberCarousel] No active member found to the left");
                _isTransitioning = false;
                return;
            }
            
            _currentSelectedIndex = nextIndex;
            
            MoveBorderToIndex(_currentSelectedIndex, true, () =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            });
        }

        /// <summary>
        /// 선택을 오른쪽으로 이동 (행동한 멤버 건너뛰기)
        /// </summary>
        private void MoveSelectionRight()
        {
            _isTransitioning = true;
            
            int nextIndex = FindNextActiveIndex(_currentSelectedIndex, 1);
            
            if (nextIndex == -1)
            {
                Debug.LogWarning("[MemberCarousel] No active member found to the right");
                _isTransitioning = false;
                return;
            }
            
            _currentSelectedIndex = nextIndex;
            
            MoveBorderToIndex(_currentSelectedIndex, true, () =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            });
        }

        /// <summary>
        /// 다음 활성 멤버의 인덱스 찾기 (행동하지 않은 멤버)
        /// </summary>
        /// <param name="startIndex">시작 인덱스</param>
        /// <param name="direction">방향 (-1: 왼쪽, 1: 오른쪽)</param>
        /// <returns>다음 활성 멤버 인덱스, 없으면 -1</returns>
        private int FindNextActiveIndex(int startIndex, int direction)
        {
            int count = _allUnits.Count;
            int nextIndex = startIndex;

            for (int i = 0; i < count; i++)
            {
                nextIndex += direction;
                

                if (nextIndex < 0)
                    nextIndex = count - 1;
                else if (nextIndex >= count)
                    nextIndex = 0;

                if (nextIndex == startIndex)
                    break;

                if (!HasMemberActed(nextIndex))
                {
                    return nextIndex;
                }
            }
            
            return -1;
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
        /// 현재 선택된 멤버가 행동했는지 확인
        /// </summary>
        public bool IsCurrentMemberActed()
        {
            if (_currentSelectedIndex < 0 || _currentSelectedIndex >= _allUnits.Count)
                return false;
            
            return HasMemberActed(_currentSelectedIndex);
        }

        /// <summary>
        /// 다음 활성 멤버로 자동 전환 (현재 멤버가 행동했을 때)
        /// </summary>
        public void MoveToNextActiveMember()
        {
            if (!IsCurrentMemberActed())
            {
                Debug.Log("[MemberCarousel] Current member is still active");
                return;
            }
            
            int nextIndex = FindNextActiveIndex(_currentSelectedIndex, 1);
            
            if (nextIndex == -1)
            {
                nextIndex = FindNextActiveIndex(_currentSelectedIndex, -1);
            }
            
            if (nextIndex == -1)
            {
                Debug.LogWarning("[MemberCarousel] No active members remaining");
                return;
            }
            
            _currentSelectedIndex = nextIndex;
            
            MoveBorderToIndex(_currentSelectedIndex, true, () =>
            {
                _onMemberSelected?.Invoke(_allUnits[_currentSelectedIndex]);
            });
            
            Debug.Log($"[MemberCarousel] Auto-switched to next active member: {_allUnits[_currentSelectedIndex].unitName}");
        }

        /// <summary>
        /// 슬롯을 직접 클릭했을 때
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            if (_isTransitioning) return;
            if (slotIndex < 0 || slotIndex >= memberIconSlots.Count) return;
            if (slotIndex >= _allUnits.Count) return;

            if (HasMemberActed(slotIndex))
            {
                Debug.Log($"[MemberCarousel] Cannot select member at index {slotIndex} - already acted");
                return;
            }
            
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