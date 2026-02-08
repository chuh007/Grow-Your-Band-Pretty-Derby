using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.MainSystem.StatSystem.BaseStats
{
    [Serializable]
    public struct StatRank 
    {
        public StatRankType RankName;
        public int Threshold;
        public AssetReferenceSprite RankIconReference;
    }
    
    [CreateAssetMenu(fileName = "Stat rank", menuName = "SO/Stat/Stat rank", order = 0)]
    public class StatRankTable : ScriptableObject
    {
        public List<StatRank> Ranks = new List<StatRank>();
        
        /// <summary>
        /// 입력된 스탯 수치를 기반으로 현재 해당하는 등급의 이름을 반환한다.
        /// </summary>
        /// <param name="value">판정할 현재 스탯 값</param>
        /// <returns>해당 수치 구간의 등급 명칭, 어떤 등급 조건도 만족하지 못할 경우 "Null"을 반환한다.</returns>
        public StatRankType GetRankName(int value)
        {
            for (int i = Ranks.Count - 1; i >= 0; i--)
            {
                if (value >= Ranks[i].Threshold)
                    return Ranks[i].RankName;
            }
            return StatRankType.None;
        }
        
        /// <summary>
        /// 입력된 스탯 수치를 기반으로 현재 해당하는 등급의 아이콘을 반환한다.
        /// </summary>
        /// <param name="value">판정할 현재 스탯 값</param>
        /// <returns>해당 수치 구간의 등급 아이콘, 어떤 등급 조건도 만족하지 못할 경우 Null을 반환한다.</returns>
        public AssetReferenceSprite GetRankIcon(int value)
        {
            for (int i = Ranks.Count - 1; i >= 0; i--)
            {
                if (value >= Ranks[i].Threshold)
                    return Ranks[i].RankIconReference;
            }
            
            return null;
        }
        
        /// <summary>
        /// 다음 등급의 Threshold 값을 반환한다.
        /// </summary>
        /// <param name="currentValue">현재 스탯 값</param>
        /// <returns>다음 등급의 시작 값, 이미 최고 등급이면 null</returns>
        public int? GetNextRankThreshold(int currentValue)
        {
            // Ranks가 Threshold 오름차순으로 정렬되어 있다고 가정
            for (int i = 0; i < Ranks.Count; i++)
            {
                if (Ranks[i].Threshold > currentValue)
                {
                    return Ranks[i].Threshold;
                }
            }
            return null; // 최고 등급
        }
        
        /// <summary>
        /// 현재 등급의 시작 Threshold 값을 반환한다. (게이지 진행도 계산용)
        /// </summary>
        /// <param name="currentValue">현재 스탯 값</param>
        /// <returns>현재 등급의 시작 값</returns>
        public int? GetCurrentRankThreshold(int currentValue)
        {
            for (int i = Ranks.Count - 1; i >= 0; i--)
            {
                if (currentValue >= Ranks[i].Threshold)
                {
                    return Ranks[i].Threshold;
                }
            }
            return null; 
        }
        
        public async Task LoadAllRankIconsAsync()
        {
            GameResourceManager resourceManager = GameResourceManager.Instance;
            var tasks = Ranks.Where(r => r.RankIconReference.RuntimeKeyIsValid())
                .Select(r => resourceManager.LoadAsync<Sprite>(r.RankIconReference.RuntimeKey.ToString()));
            await Task.WhenAll(tasks);
        }
    }
}