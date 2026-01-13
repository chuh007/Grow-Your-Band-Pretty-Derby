using System.Linq;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Contexts;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }
        
        public int CurrentLevel { get; private set; }
        public bool IsActive { get; private set; }
        public int RemainingTurns { get; private set; }
        
        public List<float> CurrentEffects { get; private set; }
        
        public bool CanBeRemoved => Data.IsRemovable && !IsActive;
        public int Point => Data.Point;
        public string Name => Data.TraitName;
        public TraitType Type => Data.TraitType;
        
        public ActiveTrait(TraitDataSO data, int initialLevel = 1)
        {
            Data = data;
            CurrentLevel = initialLevel;
            IsActive = false;
            RemainingTurns = -1;
            
            CurrentEffects = new List<float>(data.Effects);
        }
        
        public void Activate(GameContext context)
        {
            if (IsActive)
                return;
            
            if (Data.Condition != null && !Data.Condition.IsMet(context))
                return;
            
            Data.Effect?.Apply(context);
            IsActive = true;

            if (Data.ExpirationType == ExpirationType.TurnBased)
                RemainingTurns = (int)CurrentEffects[1];
        }

        public void Deactivate(GameContext context)
        {
            if (!IsActive)
                return;
            
            Data.Effect?.Remove(context);
            IsActive = false;
        }
        
        public void LevelUp()
        {
            CurrentLevel++;
            
            for (int i = 0; i < CurrentEffects.Count; i++)
                CurrentEffects[i] *= 1.1f;
        }
        
        public bool CheckExpiration(int currentTurn, List<string> occurredEvents)
        {
            switch (Data.ExpirationType)
            {
                case ExpirationType.TurnBased:
                    if (RemainingTurns > 0)
                    {
                        RemainingTurns--;
                        if (RemainingTurns == 0)
                            return true;
                    }
                    break;
                case ExpirationType.EventBased:
                    if (occurredEvents.Any(evt => Data.DescriptionCondition.Contains(evt)))
                        return true;
                    break;
                    
                case ExpirationType.ConditionBased:
                case ExpirationType.None:
                    break;
            }
            return false;
        }
    }
}