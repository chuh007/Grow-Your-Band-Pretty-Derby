using Code.Core.Bus;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Events;

namespace Code.MainSystem.StatSystem.Stats
{
    public class TeamStat : AbstractStats
    {
        protected override void Awake()
        {
            base.Awake();
            Bus<TeamStatValueChangedEvent>.OnEvent += OnTeamStatChanged;
        }
        
        private void OnDestroy()
        {
            Bus<TeamStatValueChangedEvent>.OnEvent -= OnTeamStatChanged;
        }

        private void OnTeamStatChanged(TeamStatValueChangedEvent evt)
        {
            BaseStat stat = GetStat(evt.StatType);
            stat?.PlusValue(evt.AddValue);
        }

        public BaseStat GetTeamStat()
        {
            return _isInitialized ? GetStat(StatType.TeamHarmony) : null;
        }
    }
}
