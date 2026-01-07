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
            turnCountText.SetText((_turnManager.RemainingTurn + 1).ToString());
            turnChangeUIImage.rectTransform.anchoredPosition =
                new Vector2(0, turnChangeUIImage.rectTransform.rect.height);
            
            // 화면 내려옴
            turnChangeUIImage.rectTransform.
                DOMoveY(turnChangeUIImage.rectTransform.rect.height / 2, 0.5f);
            
            await UniTask.Delay(1000); // 딜레이 (ms 단위)
            turnCountRectTrm.DOLocalMoveY(30, 0.1f).SetEase(Ease.InSine) // 달력 위로
                .OnComplete(() =>
                {
                    turnCountText.SetText(_turnManager.RemainingTurn.ToString());
                    turnCountRectTrm.DOLocalMoveY(0, 0.15f).SetEase(Ease.OutSine); // 달력 아래로
                });
            
            await UniTask.Delay(1200); // 딜레이 (ms 단위)
            // 화면 올라감
            turnChangeUIImage.rectTransform.
                DOMoveY(-turnChangeUIImage.rectTransform.rect.height * 0.5f, 0.5f);
        }
        
        public void TurnEnd()
        {
        }

        public void TurnStart()
        {

        }
    }
}