using Characters.Inventories;
using GameSystem.Window.Menu;
using TMPro;
using UnityEngine;

namespace Menus.InventoryMenu.MenuItem
{
    public class InventoryMenuItem : MonoBehaviour, IMenuItem<InventoryData>
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemMultiplier;
        [SerializeField] private TextMeshProUGUI itemQuantity;

        public InventoryData Value { get; private set; }
        public Transform Transform => transform;
        public TextMeshProUGUI Text => itemName;

        public bool IsNotNullOrEmpty() {
            return Value != null && Value.item != null;
        }

        public void SetMenuItem(InventoryData itemData) {
            Value = itemData;

            if (itemData == null || itemData.item == null) {
                itemName.enabled = false;
                itemMultiplier.enabled = false;
                itemQuantity.enabled = false;
                return;
            }

            itemName.text = Value.Name;
            itemQuantity.text = Value.quantity.ToString();

            itemName.enabled = true;
            itemMultiplier.enabled = true;
            itemQuantity.enabled = true;
        }

        public override string ToString() {
            return $"{Value.Name} - {Value.quantity}";
        }
    }
}