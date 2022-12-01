using System.Collections;
using System.Linq;
using UnityEngine;

namespace System.Window.Menu.Grid
{
    public abstract class GridMenu<T> : MenuBase<T>
    {
        public IMenuItem<T>[,] OptionsGrid { get; protected set; }
        
        public override IEnumerator OpenWindow(Vector2 pos = default, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            if (!Initialised) Initialise();
            if (OptionsGrid == null) yield break;

            var defaultSelection = this.GetInitialMatrixPosition(false);
            CurrentOption = defaultSelection.Option;
            CurrentCursorPosition = (defaultSelection.Col, defaultSelection.Row);
            
            OnOptionChange(null, CurrentOption);
                
            SetDefaultFontColor();
            SetNewHighlightedOption(null, CurrentOption);
        
            yield return base.OpenWindow(pos, onConfirmCallback, onCancelCallback);
        }
        
        protected virtual void OnOptionChange(IMenuItem<T> previousOption, IMenuItem<T> newOption)
        {
            var (row, col) = CurrentCursorPosition;
            if (enableCursor) SetCursorPosition(row, col);
            if (enableHighlight) SetNewHighlightedOption(previousOption, CurrentOption);
        }
        
        protected override void SetCursorPosition(int x, int y)
        {
            if (!enableCursor) return;
        
            var cursorPos = OptionsGrid[x, y].Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }

        public IEnumerator RunWindow()
        {
            while (WindowOpen)
            {
                var updatedChoice = this.GetNextGridMenuOption();

                var previousOption = CurrentOption;
                CurrentCursorPosition = (updatedChoice.Col, updatedChoice.Row);
                CurrentOption = updatedChoice.Option;

                if (previousOption != CurrentOption)
                    OnOptionChange(previousOption, CurrentOption);

                if (Input.GetKeyDown(KeyCode.Z)) yield return OnConfirm();
                if (Input.GetKeyDown(KeyCode.X)) yield return OnCancel();

                yield return null;
            }
        }

        protected override IEnumerator OnClose()
        {
            Choice = CloseReason == WindowCloseReason.Complete ? CurrentOption : null;
            yield break;
        }
    
        private void ClearOptions()
        {
            foreach (var pair in OptionsGrid) {
                Destroy(pair.Transform.gameObject);
            }
        
            OptionsGrid = new IMenuItem<T>[,]{};
        }
        
        protected override void SetDefaultFontColor()
        {
            if(!enableHighlight) return;
            
            OptionsGrid.GetRowsFlattened()
                .ToList()
                .ForEach(value => value.Option.Text.color = fontColour);
        }
    }
}