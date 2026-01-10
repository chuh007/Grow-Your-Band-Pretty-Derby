using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.TraitSystem.Data
{
    public enum TraitType
    {
        Training, Performance, Etc
    }
    
    public enum ExpirationType
    {
        None, TurnBased, EventBased, ConditionBased
    }
    
    [CreateAssetMenu(fileName = "Trait data", menuName = "SO/Trait/Trait data", order = 0)]
    public class TraitDataSO : ScriptableObject
    {
        public string TraitID;
        public TraitType TraitType;
        public int Point;
        public bool IsRemove;
        public ExpirationType ExpirationType;
        public List<float> Effects;
        public int Level;
    }
}