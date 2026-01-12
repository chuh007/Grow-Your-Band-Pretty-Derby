using System;
using System.Data.Common;
using Code.MainSystem.TraitSystem.Contexts;
using Code.MainSystem.TraitSystem.Data;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Runtime
{
    public class ActiveTrait
    {
        public TraitDataSO Data { get; private set; }
        public bool IsActive { get; private set; }
        
        public bool CanBeRemoved => !Data.IsRemove;

        public ActiveTrait(TraitDataSO data)
        {
            Data = data;
            IsActive = false;
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