using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.MainSystem.StatSystem.BaseStats
{
    [Serializable]
    public struct StatRank 
    {
        public string rankName;
        public int threshold;
    }
    
    [CreateAssetMenu(fileName = "Stat rank", menuName = "SO/Stat/Stat rank", order = 0)]
    public class StatRankTable : ScriptableObject
    {
        public List<StatRank> ranks = new List<StatRank>();
        
        /// <summary>
        /// 입력된 스탯 수치를 기반으로 현재 해당하는 등급의 이름을 반환한다.
        /// </summary>
        /// <param name="value">판정할 현재 스탯 값</param>
        /// <returns>해당 수치 구간의 등급 명칭, 어떤 등급 조건도 만족하지 못할 경우 "Null"을 반환한다.</returns>
        public string GetRankName(int value)
        {
            for (int i = ranks.Count - 1; i >= 0; i--)
            {
                if (value >= ranks[i].threshold)
                    return ranks[i].rankName;
            }
            return "Null";
        }
    }
}