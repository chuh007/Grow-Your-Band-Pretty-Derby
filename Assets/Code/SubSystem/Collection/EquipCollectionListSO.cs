using System.Collections.Generic;
using UnityEngine;

namespace Code.SubSystem.Collection
{
    /// <summary>
    /// collections에 지금 장착한 소장품 넣기
    /// </summary>
    [CreateAssetMenu(fileName = "EquipCollections", menuName = "SO/Collection/List", order = 0)]
    public class EquipCollectionListSO : ScriptableObject
    {
        public List<CollectionDataSO> collections;
    }
}