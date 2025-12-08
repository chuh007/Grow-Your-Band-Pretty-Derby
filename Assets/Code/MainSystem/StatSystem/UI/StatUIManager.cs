using UnityEngine;
using System.Collections.Generic;
using Code.Core.Bus;
using Code.MainSystem.StatSystem.Events;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.StatSystem.MemberStats;
using Code.MainSystem.StatSystem.TeamStats;

namespace Code.MainSystem.StatSystem.UI
{
    public class StatUIManager : MonoBehaviour
    {
        [SerializeField] private StatManager statManager;
        [SerializeField] private TeamStatPanel teamStatPanel;
        [SerializeField] private List<MemberStatPanel> memberStatPanels;
        
        private void Start()
        {
            InitializeUI();
            SubscribeEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeEvents();
        }
        
        private void InitializeUI()
        {
            teamStatPanel.EnableFor(statManager.GetComponent<TeamStat>());
            
            var members = statManager.GetComponentsInChildren<MemberStat>();
            for (int i = 0; i < members.Length && i < memberStatPanels.Count; i++)
                memberStatPanels[i].EnableFor(members[i]);
        }
        
        private void SubscribeEvents()
        {
            Bus<StatUpgradeEvent>.OnEvent += OnStatUpgraded;
            Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
        }
        
        private void UnsubscribeEvents()
        {
            Bus<StatUpgradeEvent>.OnEvent -= OnStatUpgraded;
            Bus<TeamStatValueChangedEvent>.OnEvent -= OnTeamStatChanged;
        }
        
        private void OnStatUpgraded(StatUpgradeEvent evt)
        {
            if (evt.Upgrade)
                RefreshAllUI();
        }
        
        private void OnTeamStatChanged(TeamStatValueChangedEvent evt)
        {
            RefreshAllUI();
        }
        
        private void RefreshAllUI()
        {
            teamStatPanel.UpdateUI();
            
            foreach (var panel in memberStatPanels)
                panel.UpdateUI();
        }
    }
}