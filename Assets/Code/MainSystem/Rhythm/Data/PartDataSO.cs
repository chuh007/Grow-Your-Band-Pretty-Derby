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

    [CreateAssetMenu(fileName = "PartData", menuName = "RhythmGame/PartData")]
    public class PartDataSO : ScriptableObject
    {
        public PartType PartType;
        
        public float JudgementDifficulty = 1.0f; 
        
        public float ScoreMultiplier = 1.0f;

        public FeverBonusType FeverBonusType;
    }

    public enum FeverBonusType
    {
        ScoreMultiplier,
        SafeMiss,
        HealthRegen
    }
}