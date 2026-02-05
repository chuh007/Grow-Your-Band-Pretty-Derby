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
        
        public async Task LoadAllRankIconsAsync()
        {
            GameResourceManager resourceManager = GameResourceManager.Instance;
            var tasks = Ranks.Where(r => r.RankIconReference.RuntimeKeyIsValid())
                .Select(r => resourceManager.LoadAsync<Sprite>(r.RankIconReference.RuntimeKey.ToString()));
            await Task.WhenAll(tasks);
        }
    }
}