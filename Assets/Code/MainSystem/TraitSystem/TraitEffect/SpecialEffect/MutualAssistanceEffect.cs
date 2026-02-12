using Code.MainSystem.TraitSystem.Interface;
using Code.MainSystem.TraitSystem.Runtime;

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
        
        // TODO : TeamPracticeCompo 에 적용할 로직
        // var holders = _selectedMembers
        //         .Select(u => TraitManager.Instance.GetHolder(u.memberType))
        //         .Where(h => h != null)
        //         .Cast<IModifierProvider>()
        //         .ToList();
        //
        //     // 3. StatManager 싱글톤을 통해 보너스 로직 실행
        //     // out 파라미터를 사용하여 결과값들을 즉시 변수로 선언합니다.
        //     if (StatManager.Instance.ApplyLowestStatBonus(holders, 
        // out MemberType rewardedMember, 
        //     out StatType rewardedStat, 
        //     out int bonusValue))
        // {
        //     // 4. 보너스가 적용되었다면 UI 결과창용 캐시에 합산
        //     var bonusKey = (rewardedMember, rewardedStat);
        //
        //     if (!TeamPracticeResultCache.StatDeltaDict.TryAdd(bonusKey, bonusValue))
        //         TeamPracticeResultCache.StatDeltaDict[bonusKey] += bonusValue;
        // }
    }
}