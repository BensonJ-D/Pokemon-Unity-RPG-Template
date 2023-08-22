using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Window.Menu;
using System.Window.Menu.Scroll;
using GameSystem.Window.Menu;
using MyBox;
using UnityEngine;

namespace Menus.NumberSelector
{
    public class NumberSelectorMenu : ScrollMenu<int>
    {
        [Separator("Number Picker Settings")]
        [SerializeField] private NumberSelectorMenuItem selectionMenuItem;

        public void Start()
        {
            Initialise();

            OptionMenuItems = new List<IMenuItem<int>> {selectionMenuItem};
        }

        public IEnumerator OpenWindow(int minimum, int maximum,
            OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            SetMenu(minimum, maximum);
            yield return base.OpenWindow(onConfirmCallback: onConfirmCallback, onCancelCallback: onCancelCallback);
        }

        private void SetMenu(int minimum, int maximum)
        {
            OptionsList = Enumerable.Range(minimum, maximum).Reverse().ToList();
            selectionMenuItem.SetMenuItem(minimum);
            UseFirstElementAsDefault = false;
        }
    }
}