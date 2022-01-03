using System.Collections;
using System.Linq;
using UnityEngine;

namespace System.Window.Menu.GridMenu
{
    public abstract class GridMenu<T> : Menu<T>
    {
        public IMenuItem<T>[,] OptionsGrid { get; protected set; }
        
        protected override IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            if (OptionsGrid == null) yield break;

            var defaultSelection = this.GetInitialMatrixPosition(false);
            CurrentOption = defaultSelection.Option;
            CurrentCursorPosition = (defaultSelection.Col, defaultSelection.Row);
            
            OnOptionChange(null, CurrentOption);
                
            SetDefaultFontColor();
            SetNewHighlightedOption(null, CurrentOption);
        
            yield return base.ShowWindow(pos, isCloseable);

            while (WindowOpen) yield return RunWindow();
        }
        
        protected virtual void OnOptionChange(IMenuItem<T> previousOption, IMenuItem<T> newOption)
        {
            var (row, col) = CurrentCursorPosition;
            if (enableCursor) SetCursorPosition(row, col);
            if (enableHighlight) SetNewHighlightedOption(previousOption, CurrentOption);
        }

        private IEnumerator RunWindow()
        {
            var updatedChoice = this.GetNextGridMenuOption();
        
            var previousOption = CurrentOption;
            CurrentCursorPosition = (updatedChoice.Col, updatedChoice.Row);
            CurrentOption = updatedChoice.Option;

            if (previousOption != CurrentOption) 
                OnOptionChange(previousOption, CurrentOption);

            if (Input.GetKeyDown(KeyCode.Z)) yield return OnConfirm();
            if (Input.GetKeyDown(KeyCode.X) && IsCloseable) yield return OnCancel();
                
            yield return null;
        }

        protected override void OnClose(WindowCloseReason closeReason)
        {
            Choice = closeReason == WindowCloseReason.Complete ? CurrentOption : null;
            // Debug.Log($"Close reason {closeReason}");
            // Debug.Log($"Choice {Choice.ToString()}");
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