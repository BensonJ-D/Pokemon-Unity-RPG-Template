using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.GridMenu;
using MyBox;
using UnityEngine;

namespace Menus.StringGridMenu
{ 
    public class StringGridMenu : GridMenu<string>
    {
        [Separator("Action UI")]
        [SerializeField] private List<StringMenuItem> menuItems;
        [SerializeField] private int rows;
        [SerializeField] private int cols;

        
        public void Start()
        {
            Initialise();

            OptionsGrid = new IMenuItem<string>[rows, cols];
            var row = 0;
            var col = 0;

            menuItems.ForEach(item =>
            {
                OptionsGrid[row, col] = item;
                
                if(OptionsGrid[row, col] != null) 
                    OptionsGrid[row, col].SetMenuItem(item.Text.text);
                
                if (++row != rows) return;
                
                row = 0;
                col++;
            });

            StartCoroutine(OpenWindow());
        }
    }
}