using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.MainSystem.Turn.UI
{
    public class TurnUIController : MonoBehaviour, ITurnEndComponent, ITurnStartComponent
    {
        [SerializeField] private Image turnChangeUIImage;
        [SerializeField] private RectTransform turnCountRectTrm;
        [SerializeField] private TextMeshProUGUI turnCountText;
        
        [Inject] private TurnManager _turnManager;
        
        private void Awake()
        {
            turnChangeUIImage.rectTransform.anchoredPosition =
                new Vector2(0, turnChangeUIImage.rectTransform.rect.height);
        }

        public async UniTask TurnChangeAnimation()
        {
            turnChangeUIImage.rectTransform.anchoredPosition =
                new Vector2(0, turnChangeUIImage.rectTransform.rect.height);
            turnChangeUIImage.rectTransform.DOMoveY(turnChangeUIImage.rectTransform.rect.height / 2, 0.5f);
            await UniTask.Delay(1000);
            turnCountRectTrm.DOLocalMoveY(50, 0.3f)
                .OnComplete(() =>
                {
                    turnCountText.SetText((_turnManager.RemainingTurn - 1).ToString());
                    turnCountRectTrm.DOLocalMoveY(-50, 0.3f);
                });
            await UniTask.Delay(1000);
            turnChangeUIImage.rectTransform.DOMoveY(-turnChangeUIImage.rectTransform.rect.height, 0.5f);
        }
        
        public void TurnEnd()
        {
        }

        public void TurnStart()
        {

        }
    }
}