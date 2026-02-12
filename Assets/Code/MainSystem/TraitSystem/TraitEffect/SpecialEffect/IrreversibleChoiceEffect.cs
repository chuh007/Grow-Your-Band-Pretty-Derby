using Code.MainSystem.StatSystem.BaseStats;
using Code.MainSystem.StatSystem.Manager;
using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.TraitEffect.SpecialEffect
{
    /// <summary>
    /// 되돌릴 수 없는 선택 효과
    /// </summary>
    public class IrreversibleChoiceEffect : MultiStatModifierEffect, IConditionModifier, ITraitLifecycleListener
    {
        private float _maxStatBonusPercent;
        private MemberType _ownerMember;

        public override void Initialize(ActiveTrait trait)
        {
            base.Initialize(trait);
            _maxStatBonusPercent = GetValue(0);
        }
        
        public float ConditionCostMultiplier => 1f;
        public float ConditionRecoveryMultiplier => 0f;

        public void OnTraitAdded(MemberType member)
        {
            _ownerMember = member;
            ModifyStatMaxValues(true);
            Debug.Log($"[IrreversibleChoice] {member}의 전공 스탯 최대치가 확장되었습니다.");
        }

        public void OnTraitRemoved(MemberType member)
        {
            ModifyStatMaxValues(false);
            Debug.Log($"[IrreversibleChoice] {member}의 전공 스탯 최대치가 원복되었습니다.");
        }

        private void ModifyStatMaxValues(bool isAdding)
        {
            var statManager = StatManager.Instance;
            if (statManager == null) return;
            
            StatType[] majorStats = GetMajorStatsByMember(_ownerMember);
            if (majorStats == null) return;

            foreach (var type in majorStats)
            {
                var stat = statManager.GetMemberStat(_ownerMember, type);
                if (stat == null) continue;
                
                int amount = Mathf.RoundToInt(stat.MaxValue * _maxStatBonusPercent);
                
                if (isAdding)
                    stat.AddMaxValue(amount);
                else
                    stat.SubtractMaxValue(amount);
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