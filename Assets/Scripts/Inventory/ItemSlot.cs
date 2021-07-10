using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemMultiplier; 
        [SerializeField] private Text itemQuantity;

        public Item Item { get; private set; }

        public void SetEmpty()
        {
            Item = null;
            itemName.enabled = false;
            itemMultiplier.enabled = false;
            itemQuantity.enabled = false;
        }
        
        public void SetItem(InventoryData itemData)
        {
            Item = itemData.item;
            itemName.text = Item.Name;
            itemQuantity.text = itemData.quantity.ToString();
            
            itemName.enabled = true;
            itemMultiplier.enabled = true;
            itemQuantity.enabled = true;
        }
        
        public void SetCancel()
        {
            Item = null;
            itemName.text = "Cancel";
            
            itemName.enabled = true;
            itemMultiplier.enabled = false;
            itemQuantity.enabled = false;
        }
    }
}
