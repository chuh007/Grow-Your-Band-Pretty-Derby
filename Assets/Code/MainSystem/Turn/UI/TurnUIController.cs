using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MainSystem.Turn.UI
{
    public enum DayOfWeek
    {
        일요일 = 0, 월요일 = 1, 화요일 = 2, 수요일 = 3, 목요일 = 4, 금요일 = 5, 토요일 = 6
    }

    public class TurnUIController : MonoBehaviour
    {
        [SerializeField] private RectTransform turnCountRectTrm;
        [SerializeField] private GameObject dayOfWeekPrefab;
        [SerializeField] private TextMeshProUGUI remainingTurnText;
        
        [Header("Animation Settings")]
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float daySpacing = 450f;
        [SerializeField] private Vector3 activeScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private Vector3 inactiveScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        [Inject] private TurnManager _turnManager;
        
        private readonly List<DayOfWeekUI> _spawnedDays = new List<DayOfWeekUI>();
        private int _currentDayDisplayCount = 0;
        private bool _isAnimating = false;
        
        private static readonly string RESULT_FORMAT = "다믕 목표까지 {0}일"; 
        
        private void Start()
        {
            InitUI();
            gameObject.SetActive(false);
        }

        private void InitUI()
        {
            foreach (var day in _spawnedDays) if(day != null) Destroy(day.gameObject);
            _spawnedDays.Clear();
            _currentDayDisplayCount = 0; 
            
            CreateDayUI(_currentDayDisplayCount, Vector2.zero, activeScale);
            CreateDayUI(_currentDayDisplayCount + 1, new Vector2(daySpacing, 0), inactiveScale);
            remainingTurnText.SetText(string.Format(RESULT_FORMAT, _turnManager.RemainingTurn));
        }

        public async UniTask TurnChangeAnimation()
        {
            // 지금 일자는 크고, 나머지 일자는 작은 상태가 기본으로.
            // 호출시 다음 날로 변경되는 애니메이션 작성.
            // 지금 날짜는 크기가 작아지며 왼쪽으로 이동. 다음 날짜는 크기가 커지며 왼쪽으로 이동함.
            // 최종적으로 다음 날짜가 중앙에 위치하게 되며, 다음 날짜 뒤로는 계속 요일이 있음.
            // 일요일 0일부터 시작함.
            if (_isAnimating) return;
            _isAnimating = true;
            gameObject.SetActive(true);
            
            await UniTask.WaitForSeconds(0.25f);
            
            Vector2 lastPos = _spawnedDays[^1].RectTransform.anchoredPosition;
            CreateDayUI(_currentDayDisplayCount + 2, new Vector2(lastPos.x + daySpacing, 0), inactiveScale);
            
            _currentDayDisplayCount++;
            
            Sequence seq = DOTween.Sequence();
            
            seq.Join(turnCountRectTrm.DOAnchorPosX(turnCountRectTrm.anchoredPosition.x - daySpacing, duration).SetEase(Ease.OutQuart));
            
            for (int i = 0; i < _spawnedDays.Count; i++)
            {
                if ((i == 2 && _currentDayDisplayCount != 1) || (_currentDayDisplayCount == 1 && i == 1)) 
                    seq.Join(_spawnedDays[i].transform.DOScale(activeScale, duration));
                else 
                    seq.Join(_spawnedDays[i].transform.DOScale(inactiveScale, duration));
            }
            
            await seq.Play().AsyncWaitForCompletion();
            
            remainingTurnText.SetText(string.Format(RESULT_FORMAT, _turnManager.RemainingTurn));
            
            await UniTask.WaitForSeconds(0.25f);
            
            if (_spawnedDays.Count > 3) 
            {
                var oldDay = _spawnedDays[0];
                _spawnedDays.RemoveAt(0);
                Destroy(oldDay.gameObject);
            }
            
            turnCountRectTrm.anchoredPosition = Vector2.zero;
            float startPosX = -daySpacing;
            for (int i = 0; i < _spawnedDays.Count; i++)
            {
                _spawnedDays[i].RectTransform.anchoredPosition = new Vector2(startPosX + (i * daySpacing), 0);
            }
            
            _isAnimating = false;
            gameObject.SetActive(false);
        }

        private void CreateDayUI(int dayCount, Vector2 anchoredPos, Vector3 scale)
        {
            var go = Instantiate(dayOfWeekPrefab, turnCountRectTrm);
            var ui = go.GetComponent<DayOfWeekUI>();
            ui.RectTransform.anchoredPosition = anchoredPos;
            ui.transform.localScale = scale;

            DayOfWeek dayEnum = (DayOfWeek)(dayCount % 7);
            ui.SetDayText(dayCount + 1);
            ui.SetDayOfWeekText(dayEnum.ToString());

            _spawnedDays.Add(ui);
        }
    }
}