using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    public class PracticeCommentResult
    {
        public string practiceName;
        public string commentText;
        public Sprite resultIcon;
        public string statName;
        public int deltaValue;

        public PracticeCommentResult(string practiceName, string commentText, Sprite resultIcon, string statName, int deltaValue)
        {
            this.practiceName = practiceName;
            this.commentText = commentText;
            this.resultIcon = resultIcon;
            this.statName = statName;
            this.deltaValue = deltaValue;
        }
    }
}