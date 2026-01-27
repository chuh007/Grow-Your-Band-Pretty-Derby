using System.Collections.Generic;
using Code.Core;
using UnityEngine;

namespace Code.MainSystem.MainScreen.Training
{
    [System.Serializable]
    public class StatChangeInfo
    {
        public string statName; //스탯이름
        public int delta; // 스탯 올라간값
        public Sprite icon; // 스탯아이콘

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
        public string title; // 코멘트 타이틀 ex : 무슨무슨 특성 발동
        public string content; // 코멘트 대사
        public List<StatChangeInfo> statChanges; // 스탯 값 바뀐거 넣는거
        public string thoughts; // 소감
        public string memberName; // 멤버이름
        public PracticenType trainingType; // 훈련 타입

        public CommentData(string title, string content, List<StatChangeInfo> statChanges, 
            PracticenType trainingType, string thoughts = "", 
            string memberName = "") 
        {
            this.title = title;
            this.content = content;
            this.statChanges = statChanges;
            this.thoughts = thoughts;
            this.memberName = memberName; 
        }
    }


}