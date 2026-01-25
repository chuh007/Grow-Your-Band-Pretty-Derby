using System;
using Code.MainSystem.StatSystem.Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.SubSystem.Collection.UI
{
    public class CellInitializer : MonoBehaviour
    {
        [SerializeField] private CollectionDatabaseSO collectionDatabase;
        [SerializeField] private EquipCollectionListSO equipCollection;

        [SerializeField] private GameObject spawnParent;
        [SerializeField] private GameObject cellPrefab;
        
        [SerializeField] GridLayoutGroup grid;
        
        public void RefreshCells(MemberType memberType)
        {
            CollectionCell[] cells = spawnParent.GetComponentsInChildren<CollectionCell>();
            
            for (int i = 0; i < cells.Length; i++)
            {
                CollectionDataSO data = null;
                if (i < collectionDatabase.collections[memberType].Count)
                    data = collectionDatabase.collections[memberType][i];
                
                cells[i].SetCollectionData(data,
                    !equipCollection.collections.Contains(data));
                
                bool isEquipped = equipCollection.collections.Contains(cells[i].GetCollectionData());
            }
        }
        
        public void SetSize(int count)
        {
            int rowCount = (count - 1) / grid.constraintCount + 1;
            for (int i = 0; i < rowCount * grid.constraintCount; i++)
            {
                GameObject cell = Instantiate(cellPrefab, spawnParent.transform);
            }
        }
    }
}