using UnityEngine;

namespace Code.MainSystem.Rhythm.Data
{
    public enum PartType
    {
        Drum,
        Bass,
        Guitar,
        Keyboard,
        Vocal
    }

    [CreateAssetMenu(fileName = "PartData", menuName = "SO/Rhythm/PartData")]
    public class PartDataSO : ScriptableObject
    {
        public PartType partType;
        
        public float judgementDifficulty = 1.0f; 
        
        public float scoreMultiplier = 1.0f;

        public FeverBonusType feverBonusType;
    }

    public enum FeverBonusType
    {
        ScoreMultiplier,
        SafeMiss,
        HealthRegen
    }
}