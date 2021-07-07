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

        public InventoryData(Item item)
        {
            this.item = item;
            this.quantity = 1;
        }
        
        public InventoryData(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
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
            var index = items.FindIndex(item => Equals(item, itemData));
            
            if (index == -1) {
                items.Add(itemData);
            } else {
                items[index].quantity += itemData.quantity;
            }
        }
        
        public void Remove(InventoryData itemData)
        {
            var index = items.FindIndex(item => Equals(item, itemData));

            if (index == -1) return;
            
            items[index].quantity -= itemData.quantity;
            if (items[index].quantity < 1) {
                items.RemoveAt(index);
            }
        }
    }
}

