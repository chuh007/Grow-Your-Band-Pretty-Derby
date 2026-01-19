using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    public class PointCalculator : MonoBehaviour, IPointCalculator
    {
        public int CalculateTotalPoints(IEnumerable<ActiveTrait> traits)
        {
            return traits.Sum(t => t.Data.Point);
        }
    
        public int CalculateAfterRemoval(ITraitHolder holder, ActiveTrait toRemove)
        {
            return holder.TotalPoint - toRemove.Data.Point;
        }
    
        public int CalculateAfterAdd(ITraitHolder holder, TraitDataSO toAdd)
        {
            return holder.TotalPoint + toAdd.Point;
        }
    }
}