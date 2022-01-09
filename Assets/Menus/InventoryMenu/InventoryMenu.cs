using System.Collections;
using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.ScrollMenu;
using Battle;
using Battle.SubSystems.Party;
using Inventory;
using MyBox;
using Player;
using UnityEngine;
using UnityEngine.UI;
using VFX;

namespace Menus.InventoryMenu
{
    public class InventoryMenu : ScrollMenu<InventoryData>
    {
        [Separator("Inventory Menu Settings")]
        [SerializeField] private ItemDetails itemDetails;
        [SerializeField] private List<InventoryMenuItem> itemSlots;

        [Separator("Test data")] 
        [SerializeField] private bool useTestData;
        [SerializeField] private List<InventoryData> testInventory;
        
        public void Start()
        {
            Initialise();

            OptionMenuItems = new List<IMenuItem<InventoryData>>();
            itemSlots.ForEach(slot => OptionMenuItems.Add(slot));

            if (!useTestData) return;
            
            OptionsList = testInventory;
            StartCoroutine(OpenWindow());
        }
        
        protected override void OnOptionChange(IMenuItem<InventoryData> previousOption, IMenuItem<InventoryData> newOption, bool cursorShifted)
        {
            base.OnOptionChange(previousOption, newOption, cursorShifted);
            itemDetails.SetItemDetails(newOption.Value.item);
        }
    }
}
