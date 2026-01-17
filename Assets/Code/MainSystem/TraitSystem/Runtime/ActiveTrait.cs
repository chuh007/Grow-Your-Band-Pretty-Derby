using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;

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
        
        public void LevelUp()
        {
            CurrentLevel++;
            
            for (int i = 0; i < CurrentEffects.Count; i++)
                CurrentEffects[i] *= 1.1f;
        }
    }
}