using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.Scroll.Popup;
using MyBox;
using UnityEngine;

namespace Menus.Party
{ 
    public class PartyPopupMenu : PopupMenu<PartyPopupMenuOption>
    {
        [Separator("Party Menu Action Items")] 
        [SerializeField] private List<PartyPopupMenuItem> menuItems;

        [Separator("Test data")] 
        [SerializeField] private bool useTestData;
        [SerializeField] private List<PartyPopupMenuOption> exampleItems;
        
        public override void Start()
        {
            PopupMenuItems = new List<IMenuItem<PartyPopupMenuOption>>();
            menuItems.ForEach(menuItem => PopupMenuItems.Add(menuItem));
            
            base.Start();
            
            // if(useTestData) StartCoroutine(ShowWindow(exampleItems));
        }
    }
}