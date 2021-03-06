using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.Grid;
using MyBox;
using UnityEngine;

namespace Menus.Action
{ 
    public class ActionMenu : GridMenu<ActionMenuOption>
    {
        [Separator("Action UI")]
        [SerializeField] private List<ActionMenuItem> menuItems;
        
        public void Start()
        {
            Initialise();
            
            OptionsGrid = new IMenuItem<ActionMenuOption>[,]
            {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };
            
            StartCoroutine(OpenWindow());
        }
    }
}