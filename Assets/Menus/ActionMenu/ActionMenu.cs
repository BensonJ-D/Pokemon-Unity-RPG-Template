using System.Collections;
using System.Collections.Generic;
using System.Window;
using System.Window.Menu;
using System.Window.Menu.Grid;
using Menus.Action;
using MyBox;
using UnityEngine;

namespace Menus.ActionMenu
{ 
    public class ActionMenu : GridMenu<ActionMenuOption>
    {
        [Separator("Action UI")]
        [SerializeField] private List<ActionMenuItem> menuItems;

        private bool _cancellable;

        public override void Initialise()
        {
            OptionsGrid = new IMenuItem<ActionMenuOption>[,]
            {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };
            
            base.Initialise();
        }
        
        public IEnumerator OpenWindow(bool cancellable = true, Vector2 pos = default, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            _cancellable = cancellable;
            yield return base.OpenWindow(pos, onConfirmCallback, onCancelCallback);
        }

        protected override IEnumerator OnConfirm()
        {
            Choice = CurrentOption;
            CloseReason = WindowCloseReason.Complete;
            yield return base.OnConfirm();
        }
        
        protected override IEnumerator OnCancel()
        {
            if (!_cancellable) yield break;
            
            CloseReason = WindowCloseReason.Cancel;
            yield return base.OnCancel();
        }
    }
}