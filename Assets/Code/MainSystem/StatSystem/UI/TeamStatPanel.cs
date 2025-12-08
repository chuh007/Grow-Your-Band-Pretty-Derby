using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.TeamStats;

namespace Code.MainSystem.StatSystem.UI
{
    public class TeamStatPanel : MonoBehaviour, IUIElement<TeamStat>
    {
        [SerializeField] private Transform statContainer;
        [SerializeField] private StatUI statUIPrefab;
        
        private TeamStat _teamStat;
        private readonly List<StatUI> _statUIs = new();
        
        public void EnableFor(TeamStat stats)
        {
            _teamStat = stats;
            gameObject.SetActive(stats != null);
            
            if (_teamStat != null)
            {
                ClearStatUIs();
                SetupStatUIs();
            }
        }
        
        public void UpdateUI()
        {
            if (_teamStat != null)
            {
                foreach (var statUI in _statUIs)
                    statUI.UpdateUI();
            }
        }
        
        private void SetupStatUIs()
        {
            CreateStatUI(StatType.TeamHarmony);
        }
        
        private void CreateStatUI(StatType statType)
        {
            BaseStat stat = _teamStat.GetTeamStat(statType);
            if (stat != null)
            {
                StatUI ui = Instantiate(statUIPrefab, statContainer);
                ui.EnableFor(stat);
                _statUIs.Add(ui);
            }
        }
        
        private void ClearStatUIs()
        {
            foreach (var statUI in _statUIs)
                Destroy(statUI.gameObject);
            
            _statUIs.Clear();
        }
    }
}