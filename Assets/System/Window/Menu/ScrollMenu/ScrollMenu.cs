using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Window.Menu.ScrollMenu
{
    public abstract class ScrollMenu<T> : Menu<T>
    {
        public List<IMenuItem<T>> OptionMenuItems { get; protected set; }
        public List<T> OptionsList { get; protected set; }
        public int CurrentListPosition { get; private set; }
    
        protected override IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            if (OptionsList == null) yield break;

            var defaultSelection = this.GetInitialScrollPosition();
            CurrentOption = defaultSelection.Option;
            CurrentCursorPosition = (0, defaultSelection.CursorIndex);
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

            var (_, cursorPosition) = CurrentCursorPosition;
            if (enableCursor) SetCursorPosition(cursorPosition);
            if (enableHighlight) SetNewHighlightedOption(previousOption, newOption);
        }

        private IEnumerator RunWindow()
        {
            var updatedChoice = this.GetNextScrollMenuOption();
        
            var previousOption = CurrentOption;
            var previousPosition = CurrentListPosition;
            CurrentCursorPosition = (0, updatedChoice.CursorIndex);
            CurrentListPosition = updatedChoice.ScrollIndex;
            CurrentOption = updatedChoice.Option;

            if (previousOption != CurrentOption || previousPosition != CurrentListPosition) 
                OnOptionChange(previousOption, CurrentOption, previousPosition != CurrentListPosition);

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
    
        protected override void SetDefaultFontColor()
        {
            if(!enableHighlight) return;
            
            OptionMenuItems.ForEach(value => value.Text.color = fontColour);
        }
        
        protected virtual void SetVisibleItems()
        {
            var (_, cursorPosition) = CurrentCursorPosition;
            for (var i = 0; i < OptionMenuItems.Count; i++)
            {
                var listIndex = CurrentListPosition - cursorPosition + i;
                if (listIndex >= OptionsList.Count) { 
                    OptionMenuItems[i].SetMenuItem(default);
                } else {
                    OptionMenuItems[i].SetMenuItem(OptionsList[listIndex]);
                }
            }
        }
    }
}