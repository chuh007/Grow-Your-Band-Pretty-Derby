using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.MainSystem.StatSystem.Stats
{
    public class MemberStat : AbstractStats
    {
        [SerializeField] private MemberType memberType;
        
        public MemberType MemberType => memberType;
    }
}