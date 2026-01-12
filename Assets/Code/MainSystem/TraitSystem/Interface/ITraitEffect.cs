using Code.MainSystem.TraitSystem.Contexts;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitEffect
    {
        /// <summary>
        /// 정보를 받아서 실제 효과 적용
        /// </summary>
        /// <param name="context">헤당 정보</param>
        void Apply(GameContext context);
        /// <summary>
        /// 효과 제거
        /// </summary>
        /// <param name="context">헤당 정보</param>
        void Remove(GameContext context);
    }
}