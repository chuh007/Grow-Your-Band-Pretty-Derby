using System.Collections.Generic;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    /// <summary>
    /// 플레이어가 소지한 모든 소장품이 저장됨
    /// </summary>
    [CreateAssetMenu(fileName = "CollectionDatabase", menuName = "SO/Collection/Database", order = 0)]
    public class CollectionDatabaseSO : ScriptableObject
    {
        public List<CollectionDataSO> collections = new List<CollectionDataSO>();
    }
}