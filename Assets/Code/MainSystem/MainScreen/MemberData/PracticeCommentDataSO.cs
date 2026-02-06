using UnityEngine;

namespace Code.MainSystem.MainScreen.MemberData
{
    [CreateAssetMenu(fileName = "TeamPracticeComment", menuName = "SO/Training/TeamPracticeComment")]
    public class PracticeCommentDataSO : ScriptableObject
    {
        [TextArea(3, 10)]
        public string comment;
        
        public Sprite icon;
        
        [TextArea(2, 5)]
        public string thoughts;
    }
}