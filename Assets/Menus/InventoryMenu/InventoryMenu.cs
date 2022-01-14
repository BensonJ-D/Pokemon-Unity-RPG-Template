using System.Collections;
using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.ScrollMenu;
using Inventory;
using MyBox;
using UnityEngine;

namespace Menus.InventoryMenu
{
    public class InventoryMenu : ScrollMenu<InventoryData>
    {
        [Separator("Inventory Menu Settings")]
        [SerializeField] private ItemDetails itemDetails;
        [SerializeField] private NumberSelectorMenu.NumberSelectorMenu numberSelector;
        [SerializeField] private InventoryPopupMenu popupMenu;
        [SerializeField] private List<InventoryMenuItem> itemSlots;

        private Inventory.Inventory _inventory;
        
        public void Start()
        {
            base.Initialise();

            OptionMenuItems = new List<IMenuItem<InventoryData>>();
            itemSlots.ForEach(slot => OptionMenuItems.Add(slot));
        }
        
        public IEnumerator OpenWindow(Inventory.Inventory inventory)
        {
            _inventory = inventory;
            OptionsList = inventory.Items;
            yield return base.OpenWindow();
        }
        
        protected override IEnumerator OnConfirm()
        {
            var options = new List<InventoryPopupMenuOption> {InventoryPopupMenuOption.Use, InventoryPopupMenuOption.Toss};
            yield return popupMenu.OpenWindow(options, onConfirmCallback: OnPopupConfirm, onCancelCallback: () => popupMenu.CloseWindow());
            yield return popupMenu.RunWindow();
        }

        private IEnumerator OnPopupConfirm(InventoryPopupMenuOption choice)
        {
            if (choice == InventoryPopupMenuOption.Toss)
            {
                yield return popupMenu.CloseWindow();
                yield return numberSelector.OpenWindow(1,  CurrentOption.Value.quantity, OnTossConfirm, () => numberSelector.CloseWindow());
                yield return numberSelector.RunWindow();
            }

            yield return null;
        }

        private IEnumerator OnTossConfirm(int choice)
        {
            InventoryData removalData = new InventoryData(CurrentOption.Value.item, choice);
            _inventory.Remove(removalData);
            SetVisibleItems();
            
            if(_inventory.Items.Count == 0) cursor.gameObject.SetActive(false);

            yield return numberSelector.CloseWindow();
        }
        
        protected override void OnOptionChange(IMenuItem<InventoryData> previousOption, IMenuItem<InventoryData> newOption, bool cursorShifted)
        {
            base.OnOptionChange(previousOption, newOption, cursorShifted);
            itemDetails.SetItemDetails(newOption.Value.item);
        }
    }
}
