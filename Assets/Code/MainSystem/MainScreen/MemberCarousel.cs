using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Code.MainSystem.MainScreen.MemberData;
using Code.Core;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.MainScreen
{
    /// <summary>
    /// 멤버 캐러셀 UI 컨트롤러
    /// 5개의 아이콘 슬롯을 통해 멤버를 순환 선택할 수 있습니다
    /// </summary>
    public class MemberCarousel : MonoBehaviour
    {
        [Header("Member Icon Slots")]
        [SerializeField] private List<Image> memberIconSlots;
        
        [Header("Arrow Buttons")]
        [SerializeField] private Button leftArrowButton;
        [SerializeField] private Button rightArrowButton;
        
        [Header("Visual Settings")]
        [SerializeField] private Color selectedColor = Color.white;
        [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private Ease transitionEase = Ease.OutQuad;
        
        private List<UnitDataSO> _allUnits;
        private int _currentCenterIndex = 0;
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
            
            leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
            rightArrowButton.onClick.AddListener(OnRightArrowClicked);
            
            RefreshUI();
            
            if (_allUnits.Count > 0)
            {
                _onMemberSelected?.Invoke(_allUnits[_currentCenterIndex]);
            }
        }

        private void OnDestroy()
        {
            if (leftArrowButton != null)
                leftArrowButton.onClick.RemoveListener(OnLeftArrowClicked);
            if (rightArrowButton != null)
                rightArrowButton.onClick.RemoveListener(OnRightArrowClicked);
        }

        private void OnLeftArrowClicked()
        {
            if (_isTransitioning) return;
            
            ShiftLeft();
        }

        private void OnRightArrowClicked()
        {
            if (_isTransitioning) return;
            
            ShiftRight();
        }

        /// <summary>
        /// 왼쪽으로 이동 (현재 캐릭터는 맨 뒤로, 맨 뒤 캐릭터가 앞으로)
        /// </summary>
        private void ShiftLeft()
        {
            _isTransitioning = true;
            
            _currentCenterIndex = (_currentCenterIndex - 1 + _allUnits.Count) % _allUnits.Count;
            
            AnimateTransition(() =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentCenterIndex]);
            });
        }
        
        private void ShiftRight()
        {
            _isTransitioning = true;
            
            _currentCenterIndex = (_currentCenterIndex + 1) % _allUnits.Count;
            
            AnimateTransition(() =>
            {
                _isTransitioning = false;
                _onMemberSelected?.Invoke(_allUnits[_currentCenterIndex]);
            });
        }

        /// <summary>
        /// UI 전체 새로고침 (애니메이션 없이)
        /// </summary>
        private void RefreshUI()
        {
            for (int i = 0; i < memberIconSlots.Count; i++)
            {
                int unitIndex = GetUnitIndexForSlot(i);
                UpdateSlot(i, unitIndex, false);
            }
        }

        /// <summary>
        /// 애니메이션과 함께 UI 업데이트
        /// </summary>
        private void AnimateTransition(System.Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();
            
            foreach (var slot in memberIconSlots)
            {
                if (slot != null)
                {
                    sequence.Join(slot.DOFade(0f, transitionDuration * 0.5f).SetEase(transitionEase));
                }
            }
            
            sequence.AppendCallback(() =>
            {
                for (int i = 0; i < memberIconSlots.Count; i++)
                {
                    int unitIndex = GetUnitIndexForSlot(i);
                    UpdateSlotData(i, unitIndex);
                }
            });
            
            foreach (var slot in memberIconSlots)
            {
                if (slot != null)
                {
                    sequence.Join(slot.DOFade(1f, transitionDuration * 0.5f).SetEase(transitionEase));
                }
            }
            
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// 슬롯 인덱스에 해당하는 유닛 인덱스 계산
        /// </summary>
        private int GetUnitIndexForSlot(int slotIndex)
        {
            // 중앙(슬롯 2)을 기준으로 좌우 2개씩 배치
            // 슬롯 인덱스: 0, 1, 2(중앙), 3, 4
            // 오프셋:      -2, -1, 0, 1, 2
            int offset = slotIndex - 2;
            int unitIndex = (_currentCenterIndex + offset + _allUnits.Count) % _allUnits.Count;
            return unitIndex;
        }

        /// <summary>
        /// 개별 슬롯 업데이트 (스프라이트 + 색상)
        /// </summary>
        private void UpdateSlot(int slotIndex, int unitIndex, bool animate = true)
        {
            if (slotIndex < 0 || slotIndex >= memberIconSlots.Count) return;
            if (unitIndex < 0 || unitIndex >= _allUnits.Count) return;
            
            var slot = memberIconSlots[slotIndex];
            if (slot == null) return;
            
            var unit = _allUnits[unitIndex];
            
            LoadAndSetSprite(slot, unit);
            
            bool isCenter = slotIndex == 2;
            Color targetColor = isCenter ? selectedColor : unselectedColor;
            
            if (animate)
            {
                slot.DOColor(targetColor, transitionDuration).SetEase(transitionEase);
            }
            else
            {
                slot.color = targetColor;
            }
        }

        /// <summary>
        /// 슬롯 데이터만 업데이트 (애니메이션 중에 사용)
        /// </summary>
        private void UpdateSlotData(int slotIndex, int unitIndex)
        {
            if (slotIndex < 0 || slotIndex >= memberIconSlots.Count) return;
            if (unitIndex < 0 || unitIndex >= _allUnits.Count) return;
            
            var slot = memberIconSlots[slotIndex];
            if (slot == null) return;
            
            var unit = _allUnits[unitIndex];
            
            LoadAndSetSprite(slot, unit);
            
            bool isCenter = slotIndex == 2;
            slot.color = isCenter ? selectedColor : unselectedColor;
        }

        /// <summary>
        /// 스프라이트 비동기 로드 및 설정
        /// </summary>
        private async void LoadAndSetSprite(Image targetImage, UnitDataSO unit)
        {
            if (targetImage == null || unit == null)
                return;
                
            if (string.IsNullOrEmpty(unit.spriteAddressableKey))
            {
                targetImage.sprite = null;
                return;
            }
            
            try
            {
                var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(unit.spriteAddressableKey);
                if (targetImage != null && sprite != null)
                {
                    targetImage.sprite = sprite;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load sprite for {unit.unitName}: {e.Message}");
            }
        }

        /// <summary>
        /// 특정 멤버로 직접 이동
        /// </summary>
        public void SelectMember(MemberType memberType)
        {
            if (_allUnits == null || _allUnits.Count == 0)
                return;
                
            int index = _allUnits.FindIndex(u => u.memberType == memberType);
            if (index >= 0 && index != _currentCenterIndex)
            {
                _currentCenterIndex = index;
                RefreshUI();
                _onMemberSelected?.Invoke(_allUnits[_currentCenterIndex]);
            }
        }

        /// <summary>
        /// 현재 선택된 멤버 가져오기
        /// </summary>
        public UnitDataSO GetCurrentMember()
        {
            if (_allUnits == null || _allUnits.Count == 0) return null;
            return _allUnits[_currentCenterIndex];
        }

        /// <summary>
        /// 멤버 아이콘 슬롯 클릭 시 호출 (선택 기능)
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            if (_isTransitioning) return;
            if (slotIndex < 0 || slotIndex >= memberIconSlots.Count) return;
            
            if (slotIndex != 2)
            {
                int offset = slotIndex - 2;
                if (offset < 0)
                {
                    for (int i = 0; i < Mathf.Abs(offset); i++)
                    {
                        ShiftLeft();
                    }
                }
                else
                {
                    for (int i = 0; i < offset; i++)
                    {
                        ShiftRight();
                    }
                }
            }
        }
    }
}