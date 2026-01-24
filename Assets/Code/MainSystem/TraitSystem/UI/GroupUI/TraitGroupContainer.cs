using UnityEngine;
using System.Collections.Generic;
using Code.MainSystem.TraitSystem.Runtime;
using Code.MainSystem.TraitSystem.Interface;

namespace Code.MainSystem.TraitSystem.UI.GroupUI
{
    public class TraitGroupContainer : MonoBehaviour, IUIElement<TraitGroupStatus[]>
    {
        [SerializeField] private TraitGroupItem itemPrefab;
        [SerializeField] private Transform itemRoot;

        private readonly List<TraitGroupItem> _items = new();

        public void EnableFor(TraitGroupStatus[] statuses)
        {
            EnsureItemCount(statuses.Length);

            for (int i = 0; i < _items.Count; i++)
            {
                if (i < statuses.Length)
                {
                    _items[i].EnableFor(statuses[i]);
                }
                else
                {
                    _items[i].Disable();
                }
            }
            
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            foreach (var item in _items)
                item.Disable();

            gameObject.SetActive(false);
        }

        private void EnsureItemCount(int count)
        {
            while (_items.Count < count)
            {
                TraitGroupItem item = Instantiate(itemPrefab, itemRoot);
                _items.Add(item);
            }
        }
    }
}