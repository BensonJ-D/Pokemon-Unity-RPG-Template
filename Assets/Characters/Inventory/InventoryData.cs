using System;

namespace Characters.Inventory
{
    [Serializable]
    public class InventoryData
    {
        public Item item;
        public int quantity;

        public string Name => item == null ? null : item.Name;

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
}