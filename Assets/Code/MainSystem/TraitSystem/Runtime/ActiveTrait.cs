using Code.MainSystem.TraitSystem.Contexts;
using Code.MainSystem.TraitSystem.Data;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }
        public int RemainingTurns;
        public bool IsActive { get; private set; }
        
        public bool CanBeRemoved => !Data.IsRemove;

        public ActiveTrait(TraitDataSO data)
        {
            Data = data;
            IsActive = false;
        }

        public void OnTurnStart(GameContext context)
        {
            if (Data.ExpirationType != ExpirationType.TurnBased)
                return;
            RemainingTurns--;
            if (RemainingTurns <= 0)
            {
                // 제거 로직 요청
            }
        }

        public void Activate(GameContext context)
        {
            if (IsActive)
                return;

            if (Data.Condition != null && !Data.Condition.IsMet(context))
                return;
            Data.Effect?.Apply(context);
            IsActive = true;
        }

        public void Deactivate(GameContext context)
        {
            if (!IsActive)
                return;
            
            Data.Effect?.Remove(context);
            IsActive = false;
        }
    }
}