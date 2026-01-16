using System.Linq;
using Code.MainSystem.TraitSystem.Data;
using Code.MainSystem.TraitSystem.Interface;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Manager.SubClass
{
    /// <summary>
    /// 포인트 처리 담당
    /// </summary>
    public class TraitPointCalculator : MonoBehaviour, ITraitPointCalculator
    {
        public int CalculateTotalPoint(ITraitHolder holder)
        {
            if (holder is CharacterTrait character)
                return character.ActiveTraits.Sum(t => t.Data.Point);
            
            return 0;
        }
    }
}