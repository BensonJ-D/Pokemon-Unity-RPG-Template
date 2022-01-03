using Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.InventoryMenu
{
    public class ItemDetails : MonoBehaviour
    {
        [SerializeField] private Text itemName;
        [SerializeField] private Text description;
        [SerializeField] private Image icon;

        public void SetItemDetails(Item item)
        {
            if (itemName != null) itemName.text = item.Name;
            if (description != null) description.text = item.Description;
            if (icon != null) icon.sprite = item.Icon;
        }
    }
}