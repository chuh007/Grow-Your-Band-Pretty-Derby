using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Contexts
{
    /// <summary>
    /// 특성 판정에 필요한 모든 정보를 담아서 전달하는 객체
    /// </summary>
    public class GameContext
    {
        public CharacterTrait Owner;
        public int CurrentTurn;
        //필요시 다른 정보 추가 해야함
    }
}