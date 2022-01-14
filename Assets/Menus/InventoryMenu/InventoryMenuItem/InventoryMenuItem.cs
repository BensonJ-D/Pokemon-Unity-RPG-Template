using System.Window.Menu;
using Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.InventoryMenu
{
    public class InventoryMenuItem : MonoBehaviour, IMenuItem<InventoryData>
    {
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemMultiplier; 
        [SerializeField] private Text itemQuantity;

        public InventoryData Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => itemName;
        
        public bool IsNotNullOrEmpty() => Value != null;

        public void SetMenuItem(InventoryData itemData)
        {
            Value = itemData;

            if (itemData == null)
            {
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

        public override string ToString()
        {
            return $"{Value.Name} - {Value.quantity}";
        }
    }
}
