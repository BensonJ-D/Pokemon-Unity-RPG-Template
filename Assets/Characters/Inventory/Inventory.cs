using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Inventory
{
    [Serializable]
    public class Inventory
    {
        [SerializeField] private List<InventoryData> items;
        public List<InventoryData> Items => items;

        public void Add(InventoryData itemData)
        {
            var index = Items.FindIndex(item => Equals(item, itemData));
            
            if (index == -1) {
                Items.Add(itemData);
            } else {
                Items[index].quantity += itemData.quantity;
            }
        }
        
        public void Remove(InventoryData itemData)
        {
            var index = Items.FindIndex(item => Equals(item, itemData));

            if (index == -1) return;
            
            Items[index].quantity -= itemData.quantity;
            if (Items[index].quantity < 1) {
                Items.RemoveAt(index);
            }
        }
        
        public void Remove(Item item, int count)
        {
            var index = Items.FindIndex(data => Equals(item, data.item));

            if (index == -1) return;
            
            Items[index].quantity -= count;
            if (Items[index].quantity < 1) {
                Items.RemoveAt(index);
            }
        }
    }
}

