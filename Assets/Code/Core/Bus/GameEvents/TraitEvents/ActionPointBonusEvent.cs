namespace Code.Core.Bus.GameEvents.TraitEvents
{
    public struct ActionPointBonusEvent : IEvent
    {
        public int BonusAmount { get; }
        
        public ActionPointBonusEvent(int bonusAmount)
        {
            BonusAmount = bonusAmount;
        }
    }
}