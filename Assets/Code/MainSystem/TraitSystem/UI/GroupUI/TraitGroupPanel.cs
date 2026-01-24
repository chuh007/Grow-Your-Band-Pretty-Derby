using System.Linq;
using UnityEngine;
using DG.Tweening;
using Reflex.Attributes;
using Code.MainSystem.TraitSystem.Manager;

namespace Code.MainSystem.TraitSystem.UI.GroupUI
{
    /// <summary>
    /// 특성 그룹 UI를 관리하는 독립적인 패널
    /// </summary>
    public class TraitGroupPanel : MonoBehaviour
    {
        [SerializeField] private TraitGroupContainer container;
        [SerializeField] private Transform contentRoot;
        
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private float hiddenX = -600f;
        [SerializeField] private float shownX = 200f;

        [Inject] private TraitManager _traitManager;
        
        private RectTransform _rectTransform;
        private bool _isOpen;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;

            if (_rectTransform != null)
                _rectTransform.anchoredPosition = new Vector2(hiddenX, _rectTransform.anchoredPosition.y);

            contentRoot.gameObject.SetActive(false);
        }

        public void Open()
        {
            if (_isOpen)
                return;

            _isOpen = true;
            contentRoot.gameObject.SetActive(true);
            RefreshUI();

            _rectTransform.DOAnchorPosX(shownX, slideDuration).SetEase(Ease.OutCubic);
        }

        public void Close()
        {
            if (!_isOpen)
                return;

            _isOpen = false;

            _rectTransform.DOAnchorPosX(hiddenX, slideDuration)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    contentRoot.gameObject.SetActive(false);
                    container?.Disable();
                });
        }

        private void RefreshUI()
        {
            if (!_isOpen || _traitManager == null)
                return;

            var statuses = _traitManager.GetTeamGroupStatus();
            if (statuses == null || statuses.Count == 0)
            {
                container?.Disable();
                return;
            }

            container?.EnableFor(statuses.ToArray());
        }
    }
}