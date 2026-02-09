using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Bus;
using Code.Core.Bus.GameEvents.TurnEvents;
using Code.MainSystem.MainScreen.Training;
using Code.MainSystem.TraitSystem.Manager;
using Code.MainSystem.Turn.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MainSystem.Turn
{
    /// <summary>
    /// 턴 사이의 로직 관리해주고, 목표 관리하는 메니저
    /// </summary>
    public class FlowManager : MonoBehaviour
    {
        [SerializeField] private TurnUIController turnUIController;
        
        private List<ITurnStartComponent> _turnStartComponents = new List<ITurnStartComponent>();
        private List<ITurnEndComponent> _turnEndComponents = new List<ITurnEndComponent>();
        private void Awake()
        {
            Bus<TurnEndEvent>.OnEvent += HandleTurnEnd;
        }

        private async void Start()
        {
            await UniTask.NextFrame(); // 1프레임 기다려서 싱글턴들이 정리하는거 기다린다.
            _turnStartComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITurnStartComponent>()
                .ToList();
            _turnEndComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<ITurnEndComponent>()
                .ToList();
            Bus<CheckTurnEnd>.Raise(new CheckTurnEnd());
        }

        private void OnDestroy()
        {
            Bus<TurnEndEvent>.OnEvent -= HandleTurnEnd;
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