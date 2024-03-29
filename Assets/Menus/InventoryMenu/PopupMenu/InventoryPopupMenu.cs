using System.Collections.Generic;
using System.Window.Menu.Scroll.Popup;
using GameSystem.Window.Menu;
using MyBox;
using UnityEngine;

namespace Menus.Inventory.PopupMenu
{
    public class InventoryPopupMenu : PopupMenu<InventoryPopupMenuOption>
    {
        [Separator("Party Menu Action Items")] [SerializeField]
        private List<InventoryPopupMenuItem> menuItems;

        [Separator("Test data")] [SerializeField]
        private bool useTestData;

        [SerializeField] private List<InventoryPopupMenuOption> exampleItems;

        public override void Start() {
            PopupMenuItems = new List<IMenuItem<InventoryPopupMenuOption>>();
            menuItems.ForEach(menuItem => PopupMenuItems.Add(menuItem));

            base.Start();

            // if(useTestData) StartCoroutine(ShowWindow(exampleItems));
        }
    }
}