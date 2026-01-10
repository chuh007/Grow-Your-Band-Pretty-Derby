using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.Turn.UI;
using UnityEngine;

namespace Code.MainSystem.Turn
{
    /// <summary>
    /// 턴 사이의 로직 관리해주는 메니저
    /// </summary>
    public class FlowManager : MonoBehaviour
    {
        [SerializeField] private TurnUIController turnUIController;
        
        private List<ITurnStartComponent> _turnStartComponents = new List<ITurnStartComponent>();
        private List<ITurnEndComponent> _turnEndComponents = new List<ITurnEndComponent>();
        private void Awake()
        {
            // 좀 마음에 안들긴 하다. 근데 직렬화필드로 빼는거 귀찮음
            _turnStartComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITurnStartComponent>()
                .ToList();
            _turnEndComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITurnEndComponent>()
                .ToList();
            
            Bus<TurnEndEvent>.OnEvent += HandleTurnEnd;
        }

        private async void HandleTurnEnd(TurnEndEvent evt)
        {
            // 턴 종료시 할 일 수행.
            foreach (var compo in _turnEndComponents)
            {
                compo.TurnEnd();
            }
            // 턴 전환 연출 진행 후
            await turnUIController.TurnChangeAnimation();
            
            // 턴 시작 작업 진행
            foreach (var compo in _turnStartComponents)
            {
                compo.TurnStart();
            }
            
        }
    }
}