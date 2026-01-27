using System.Collections.Generic;
using Code.Core;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    [System.Serializable]
    public class StatChangeInfo
    {
        public string statName;
        public int delta;
        public Sprite icon;

        public StatChangeInfo(string statName, int delta, Sprite icon)
        {
            this.statName = statName;
            this.delta = delta;
            this.icon = icon;
        }
    }
    
    [System.Serializable]
    public class CommentData
    {
        public string title;
        public string content;
        public List<StatChangeInfo> statChanges;
        public Sprite icon;
        public bool isPositive;
        public string thoughts;
        public string memberName;
        public PracticenType trainingType;

        public CommentData(string title, string content, List<StatChangeInfo> statChanges, 
            PracticenType trainingType,
            Sprite icon = null, bool isPositive = true, string thoughts = "", 
            string memberName = "") 
        {
            this.title = title;
            this.content = content;
            this.statChanges = statChanges;
            this.icon = icon;
            this.isPositive = isPositive;
            this.thoughts = thoughts;
            this.memberName = memberName; 
        }
    }


}