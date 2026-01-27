namespace Code.MainSystem.TraitSystem.Interface
{
    // 행동권 추가 (반짝이는 눈, 지나친 열정)
    public interface IActionPointBonus
    {
        float Chance { get; } // N1%
        int Amount { get; }   // N2
    }

    // 영감 시스템 (실패는 성공의 어머니)
    public interface IInspirationSystem
    {
        int GainOnFailure { get; }
        int MaxInspiration { get; }
    }

    // 판정 보정 (집중력)
    public interface IJudgmentCorrection
    {
        bool CorrectMissToGood { get; }
    }
}