using System;
using Characters.Inventories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.InventoryMenu
{
    [Serializable]
    public class ItemDetails
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image icon;

        public void SetItemDetails(Item item) {
            if (itemName != null) itemName.text = item == null ? null : item.Name;
            if (description != null) description.text = item == null ? null : item.Description;
            if (icon != null) icon.sprite = item == null ? null : item.Icon;
        }
    }
}