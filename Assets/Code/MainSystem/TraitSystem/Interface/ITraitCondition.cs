using Code.MainSystem.TraitSystem.Contexts;

namespace Code.MainSystem.TraitSystem.Interface
{
    public interface ITraitCondition
    {
        /// <summary>
        /// 정보를 받아 조건 만족 여부 반환
        /// </summary>
        /// <param name="context">해당 정보</param>
        /// <returns></returns>
        bool IsMet(GameContext context);
    }
}