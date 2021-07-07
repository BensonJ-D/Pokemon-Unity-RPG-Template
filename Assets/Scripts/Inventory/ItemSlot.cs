using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemQuantity;
        private Item item;

        public Item Item => item;
        
        public void SetEmpty()
        {
            item = null;
            itemName.text = "";
            itemQuantity.text = "";
        }
        
        public void SetItem(InventoryData itemData)
        {
            item = itemData.item;
            itemName.text = item.name;
            itemQuantity.text = itemData.quantity.ToString();
        }
    }
}
