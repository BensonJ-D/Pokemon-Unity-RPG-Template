using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Menu
{
    public abstract class ScrollMenu<T> : Window
    {
        [Separator]
        [SerializeField] private bool enableHighlight;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color highlightColour;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] private Color fontColour;
    
        [SerializeField] private bool enableCursor;
        [ConditionalField(nameof(enableCursor))] [SerializeField] private MenuCursor cursor;

        public List<IMenuItem<T>> OptionMenuItems { get; protected set; }
        public List<T> OptionsList { get; protected set; }
        public int CurrentListPosition { get; private set; }
        public int CurrentCursorPosition { get; private set; }
        public IMenuItem<T> CurrentOption { get; private set; }

        public IMenuItem<T> Choice { get; private set; }
    
        protected override IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            if (OptionsList == null) yield break;

            var defaultSelection = this.GetInitialScrollPosition();
            CurrentOption = defaultSelection.Option;
            CurrentCursorPosition = defaultSelection.CursorIndex;
            CurrentListPosition = defaultSelection.ScrollIndex;
                    
            OnOptionChange(null, CurrentOption, true);
                
            SetDefaultFontColor();
            SetNewHighlightedOption(null, CurrentOption);
            
            yield return base.ShowWindow(pos, isCloseable);

            while (WindowOpen) yield return RunWindow();
        }
        
        protected virtual void OnOptionChange(IMenuItem<T> previousOption, IMenuItem<T> newOption, bool cursorShifted)
        {
            SetVisibleItems();
            
            if (!cursorShifted) return;
            
            if (enableCursor) SetCursorPosition(CurrentCursorPosition);
            if (enableHighlight) SetNewHighlightedOption(previousOption, newOption);
        }

        private IEnumerator RunWindow()
        {
            var updatedChoice = this.GetNextScrollMenuOption();
        
            var previousOption = CurrentOption;
            var previousPosition = CurrentListPosition;
            CurrentCursorPosition = updatedChoice.CursorIndex;
            CurrentListPosition = updatedChoice.ScrollIndex;
            CurrentOption = updatedChoice.Option;

            if (previousOption != CurrentOption || previousPosition != CurrentListPosition) 
                OnOptionChange(previousOption, CurrentOption, previousPosition != CurrentListPosition);

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
            foreach (var pair in OptionMenuItems) {
                Destroy(pair.Transform.gameObject);
            }

            OptionsList.Clear();
        }

        private void SetCursorPosition(int index)
        {
            if (!enableCursor) return;
        
            var cursorPos = OptionMenuItems[index].Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }
    
        private void SetDefaultFontColor()
        {
            if(!enableHighlight) return;
            
            OptionMenuItems.ForEach(value => value.Text.color = fontColour);
        }
    
        private void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next)
        {
            if(!enableHighlight) return;
        
            if(prev != null) prev.Text.color = fontColour;
            if(next != null) next.Text.color = highlightColour;
        }
        
        protected virtual void SetVisibleItems()
        {
            for (var i = 0; i < OptionMenuItems.Count; i++)
            {
                var listIndex = CurrentListPosition - CurrentCursorPosition + i;
                if (listIndex >= OptionsList.Count) { 
                    OptionMenuItems[i].SetMenuItem(default);
                } else {
                    OptionMenuItems[i].SetMenuItem(OptionsList[listIndex]);
                }
            }
        }
    }
}