using System.Collections.Generic;
using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 서로서로 도와요 효과
    /// </summary>
    public class MutualAssistanceEffect : MultiStatModifierEffect, IMultiStatModifier
    {
        public int AddValue { get; private set; }

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            AddValue = (int)GetValue(0);
        }
        
       public void ApplyEffect(MemberType memberType, Dictionary<(MemberType, StatType), int> statDeltaDict)
        {
            var statManager = StatManager.Instance;
            if (statManager == null) return;
            
            StatType[] targetStats = GetMajorStatsByMember(memberType);
            if (targetStats == null || targetStats.Length == 0) return;

            StatType lowestStatType = targetStats[0];
            float lowestValue = float.MaxValue;

            foreach (var type in targetStats)
            {
                var stat = statManager.GetMemberStat(memberType, type);
                if (stat != null && stat.CurrentValue < lowestValue)
                {
                    lowestValue = stat.CurrentValue;
                    lowestStatType = type;
                }
            }
            
            var targetStat = statManager.GetMemberStat(memberType, lowestStatType);
            if (targetStat != null)
            {
                targetStat.PlusValue(AddValue);
                
                var key = (memberType, lowestStatType);
                if (statDeltaDict.ContainsKey(key))
                    statDeltaDict[key] += AddValue;
                else
                    statDeltaDict[key] = AddValue;

                Debug.Log($"[서로서로 도와요] {memberType}의 최저 스탯 {lowestStatType}에 +{AddValue} 적용");
            }
        }

        private StatType[] GetMajorStatsByMember(MemberType memberType)
        {
            return memberType switch
            {
                MemberType.Guitar => new[] { StatType.GuitarEndurance, StatType.GuitarConcentration },
                MemberType.Drums => new[] { StatType.DrumsSenseOfRhythm, StatType.DrumsPower },
                MemberType.Bass => new[] { StatType.BassDexterity, StatType.BassSenseOfRhythm },
                MemberType.Vocal => new[] { StatType.VocalVocalization, StatType.VocalBreathing },
                MemberType.Piano => new[] { StatType.PianoDexterity, StatType.PianoStagePresence },
                _ => null
            };
        }
    }
}