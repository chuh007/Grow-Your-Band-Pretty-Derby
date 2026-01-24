using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.SubSystem.Collection.UI
{
    public class CellInitializer : MonoBehaviour
    {
        [SerializeField] private CollectionDatabaseSO collectionDatabase;

        [SerializeField] private GameObject spawnParent;
        [SerializeField] private GameObject cellPrefab;
        
        private GridLayoutGroup _grid;
        
        private void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
        }

        private void Start()
        {
            SetSize(collectionDatabase.collections.Count);
        }

        public void SetSize(int count)
        {
            int rowCount = (count - 1) / _grid.constraintCount + 1;
            for (int i = 0; i < rowCount * _grid.constraintCount; i++)
            {
                GameObject cell = Instantiate(cellPrefab, spawnParent.transform);
                if (i < count)
                    cell.GetComponent<CollectionCell>().SetCollectionData(collectionDatabase.collections[i]);
            }
        }
    }
}