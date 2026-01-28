using Code.MainSystem.Dialogue;
using UnityEngine;

namespace Code.MainSystem.Encounter
{
    public enum EncounterConditionType
    {
        TurnEnd = 0, // 턴 종료
        BuskingCaseFall = 1, // 버스킹 조건 불만족
        BuskingFall = 2, // 버스킹 클리어 실패
        BuskingSuccess = 3, // 버스킹 성공
        LiveCaseFall = 4,
        LiveFall = 5,
        LiveSuccess = 6,
        
    }
    
    [CreateAssetMenu(fileName = "EncounterData", menuName = "SO/Encounter/Data", order = 0)]
    public class EncounterDataSO : ScriptableObject
    {
        public DialogueInformationSO dialogue; // 해당하는 다이알로그
        [Range(0, 1.0f)] public float percent; // 발생 확률(1이 최대, 무조건 발생은 1로)
        public EncounterConditionType type; // 언제 나올지
    }
}