using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.SubSystems.Party;
using Inventory;
using Menu;
using Player;
using UnityEngine;
using UnityEngine.UI;
using VFX;

namespace Menus.InventoryMenu
{
    public class InventoryMenu : ScrollMenu<InventoryData>
    {
        [SerializeField] private ItemDetails itemDetails;
        [SerializeField] private List<InventoryMenuItem> itemSlots;
        [SerializeField] private List<InventoryData> testInventory;
        
        public void Start()
        {
            Initiate();

            OptionMenuItems = new List<IMenuItem<InventoryData>>();
            itemSlots.ForEach(slot => OptionMenuItems.Add(slot));
            
            OptionsList = testInventory;

            StartCoroutine(ShowWindow());
        }
        
        protected override void OnOptionChange(IMenuItem<InventoryData> previousOption, IMenuItem<InventoryData> newOption, bool cursorShifted)
        {
            base.OnOptionChange(previousOption, newOption, cursorShifted);
            itemDetails.SetItemDetails(newOption.Value.item);
        }
    }
}
