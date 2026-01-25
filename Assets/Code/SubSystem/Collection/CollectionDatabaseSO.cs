using System.Collections.Generic;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    /// <summary>
    /// 플레이어가 소지한 모든 소장품이 저장됨
    /// </summary>
    [CreateAssetMenu(fileName = "CollectionDatabase", menuName = "SO/Collection/Database", order = 0)]
    public class CollectionDatabaseSO : ScriptableObject
    {
        // 데이터 사용하는 부분
        public Dictionary<MemberType, List<CollectionDataSO>> collections =
            new Dictionary<MemberType, List<CollectionDataSO>>();
        
        // 내가 데이터를 넣는 부분
        public List<CollectionDataSO> BassDataList;
        public List<CollectionDataSO> GuitarDataList;
        public List<CollectionDataSO> DrumsDataList;
        public List<CollectionDataSO> PianoDataList;
        public List<CollectionDataSO> VocalDataList;

        public void SetCollections()
        {
            collections.Add(MemberType.Bass, new List<CollectionDataSO>());
            collections.Add(MemberType.Guitar, new List<CollectionDataSO>());
            collections.Add(MemberType.Drums, new List<CollectionDataSO>());
            collections.Add(MemberType.Piano, new List<CollectionDataSO>());
            collections.Add(MemberType.Vocal, new List<CollectionDataSO>());
            
            foreach (var data in BassDataList)
            {
                collections[MemberType.Bass].Add(data);
            }
            foreach (var data in GuitarDataList)
            {
                collections[MemberType.Guitar].Add(data);
            }
            foreach (var data in DrumsDataList)
            {
                collections[MemberType.Drums].Add(data);
            }
            foreach (var data in PianoDataList)
            {
                collections[MemberType.Piano].Add(data);
            }
            foreach (var data in VocalDataList)
            {
                collections[MemberType.Vocal].Add(data);
            }
        }
    }
}