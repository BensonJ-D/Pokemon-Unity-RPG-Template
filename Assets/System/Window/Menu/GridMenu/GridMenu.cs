using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Menu
{
    public abstract class GridMenu<T> : Window
    {
        [Separator]
        [SerializeField] private bool enableHighlight;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color highlightColour;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color fontColour;
    
        [SerializeField] private bool enableCursor;
        [ConditionalField(nameof(enableCursor))] [SerializeField] private MenuCursor cursor;

        public IMenuItem<T>[,] OptionsGrid { get; protected set; }
        public (int, int) CurrentCursorPosition { get; private set; }
        public IMenuItem<T> CurrentOption { get; private set; }

        public IMenuItem<T> Choice { get; private set; }

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

            if (Input.GetKeyDown(KeyCode.Z)) OnConfirm();
            if (Input.GetKeyDown(KeyCode.X) && IsCloseable) OnCancel();
                
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

        private void SetCursorPosition(int x, int y)
        {
            if (!enableCursor) return;
        
            var cursorPos = OptionsGrid[x, y].Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }
    
        private void SetDefaultFontColor()
        {
            if(!enableHighlight) return;
            
            OptionsGrid.GetRowsFlattened()
                .ToList()
                .ForEach(value => value.Option.Text.color = fontColour);
        }
    
        private void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next)
        {
            if(!enableHighlight) return;
        
            if(prev != null) prev.Text.color = fontColour;
            if(next != null) next.Text.color = highlightColour;
        }
    }
}