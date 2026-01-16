using System;
using Code.Core;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.OutingEvents;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Outing
{
    public class OutingPlaceSelectUI : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        
        // [SerializeField] private Image parkIcon;
        // [SerializeField] private Image martIcon;
        // [SerializeField] private Image pcIcon;
        // [SerializeField] private Image cafeIcon;
        // [SerializeField] private Image marketIcon;
        
        private float _width;
        [SerializeField] private RectTransform rect;
        
        private void Awake()
        {
            _width = rect.rect.width;
            rect.anchoredPosition = new Vector2(-_width, 0);
            Bus<OutingUnitSelectEvent>.OnEvent += HandleUnitSelect;
        }

        private void OnDestroy()
        {
            Bus<OutingUnitSelectEvent>.OnEvent -= HandleUnitSelect;
        }

        private async void HandleUnitSelect(OutingUnitSelectEvent evt)
        {
            var sprite = await GameManager.Instance.LoadAddressableAsync<Sprite>(evt.SelectedUnit.spriteAddressableKey);
            characterImage.sprite = sprite;
            rect.DOLocalMoveX(0, 0.5f);
        }
    }
}