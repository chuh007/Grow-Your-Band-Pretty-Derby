using System;
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
        
        private void Start()
        {
            SetSize(collectionDatabase.collections.Count);
        }
        
        private void OnEnable()
        {
            RefreshCells();
        }
        
        public void RefreshCells()
        {
            CollectionCell[] cells = spawnParent.GetComponentsInChildren<CollectionCell>();
            
            foreach (var cell in cells)
            {
                bool isEquipped = equipCollection.collections.Contains(cell.GetCollectionData());
                cell.UpdateEquipState(!isEquipped);
            }
        }
        
        public void SetSize(int count)
        {
            int rowCount = (count - 1) / grid.constraintCount + 1;
            for (int i = 0; i < rowCount * grid.constraintCount; i++)
            {
                GameObject cell = Instantiate(cellPrefab, spawnParent.transform);
                if (i < count)
                {
                    CollectionDataSO data = collectionDatabase.collections[i];
                    cell.GetComponent<CollectionCell>()
                        .SetCollectionData(data,
                            !equipCollection.collections.Contains(data));
                }
            }
        }
    }
}