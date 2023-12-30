using System.Collections;
using System.Collections.Generic;
using System.Window;
using GameSystem.Window.Menu;
using GameSystem.Window.Menu.Grid;
using MyBox;
using TMPro;
using UnityEngine;

namespace Menus.ActionMenu
{
    public class ActionMenu : GridMenu<ActionMenuOption>
    {
        [Separator("Action UI")] [SerializeField]
        private List<ActionMenuItem> menuItems;

        [Separator("Dialog")] [SerializeField] private TextMeshProUGUI textField;

        [SerializeField] protected bool resizeTextField;

        [ConditionalField(nameof(resizeTextField))] [SerializeField]
        private float dialogResize;

        private bool _cancellable;

        private float _previousDialogSize;

        public override void Initialise() {
            OptionsGrid = new IMenuItem<ActionMenuOption>[,] {
                {menuItems[0], menuItems[2]},
                {menuItems[1], menuItems[3]}
            };

            base.Initialise();
        }

        public IEnumerator OpenWindow(bool cancellable = true, Vector2 pos = default,
            OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null) {
            if (resizeTextField) ResizeTextField();
            _cancellable = cancellable;
            yield return base.OpenWindow(pos, onConfirmCallback, onCancelCallback);
        }

        protected override IEnumerator OnClose() {
            if (resizeTextField) ResetTextField();
            yield return base.OnClose();
        }

        private void ResizeTextField() {
            textField.text = "";
            _previousDialogSize = textField.rectTransform.rect.width;
            textField.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dialogResize);
        }

        private void ResetTextField() {
            textField.text = "";
            textField.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _previousDialogSize);
        }

        protected override IEnumerator OnConfirm() {
            Choice = CurrentOption;
            CloseReason = WindowCloseReason.Complete;
            yield return base.OnConfirm();
        }

        protected override IEnumerator OnCancel() {
            if (!_cancellable) yield break;

            CloseReason = WindowCloseReason.Cancel;
            yield return base.OnCancel();
        }
    }
}