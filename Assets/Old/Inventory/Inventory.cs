using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public class InventoryData
    {
        public Item item;
        public int quantity;

        public string Name => item.Name;

        public InventoryData(Item item)
        {
            this.item = item;
            quantity = 1;
        }
        
        public InventoryData(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            InventoryData data = (InventoryData) obj;
            return Equals(item, data.item);
        }

        protected bool Equals(InventoryData other)
        {
            return Equals(item, other.item);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((item != null ? item.GetHashCode() : 0) * 397) ^ quantity;
            }
        }
    }

    public class Inventory : MonoBehaviour
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

