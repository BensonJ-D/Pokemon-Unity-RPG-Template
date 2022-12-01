using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace System.Window.Menu.Scroll
{
    public abstract class ScrollMenu<T> : MenuBase<T>
    {
        protected bool UseFirstElementAsDefault = true;
        
        public List<IMenuItem<T>> OptionMenuItems { get; protected set; }
        public List<T> OptionsList { get; protected set; }
        public int CurrentListPosition { get; private set; }
    
        public override IEnumerator OpenWindow(Vector2 pos = default, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            if (!Initialised) Initialise();
            if (OptionsList == null) yield break;
            
            _onConfirm = onConfirmCallback;
            _onCancel = onCancelCallback;

            var defaultSelection = this.GetInitialScrollPosition(UseFirstElementAsDefault);
            CurrentOption = defaultSelection.Option;
            CurrentCursorPosition = (0, defaultSelection.CursorIndex);
            CurrentListPosition = defaultSelection.ScrollIndex;
                    
            OnOptionChange(null, CurrentOption, true);
                
            SetDefaultFontColor();
            SetNewHighlightedOption(null, CurrentOption);
            
            yield return base.OpenWindow(pos, onConfirmCallback, onCancelCallback);
        }
        
        protected virtual void OnOptionChange(IMenuItem<T> previousOption, IMenuItem<T> newOption, bool cursorShifted)
        {
            UpdateVisibleItems();
            
            if (!cursorShifted) return;

            var (_, cursorPosition) = CurrentCursorPosition;
            if (enableCursor) SetCursorPosition(cursorPosition);
            if (enableHighlight) SetNewHighlightedOption(previousOption, newOption);
        }

        public IEnumerator RunWindow()
        {
            while (WindowOpen)
            {
                var updatedChoice = this.GetNextScrollMenuOption();

                var previousOption = CurrentOption;
                var previousPosition = CurrentListPosition;
                CurrentCursorPosition = (0, updatedChoice.CursorIndex);
                CurrentListPosition = updatedChoice.ScrollIndex;
                CurrentOption = updatedChoice.Option;

                if (previousOption != CurrentOption || previousPosition != CurrentListPosition)
                    OnOptionChange(previousOption, CurrentOption, previousPosition != CurrentListPosition);

                if (Input.GetKeyDown(KeyCode.Z) && CurrentOption.IsNotNullOrEmpty()) yield return OnConfirm();
                if (Input.GetKeyDown(KeyCode.X)) yield return OnCancel();

                yield return null;
            }
        }

        protected override IEnumerator OnClose()
        {
            Choice = CloseReason == WindowCloseReason.Complete ? CurrentOption : null;
            yield break;
        }
        
        private void SetCursorPosition(int index)
        {
            if (!enableCursor) return;

            if (!OptionMenuItems[index].IsNotNullOrEmpty())
            {
                cursor.gameObject.SetActive(false);
                return;
            }
            
            if (!cursor.gameObject.activeSelf) cursor.gameObject.SetActive(true);
        
            var cursorPos = OptionMenuItems[index].Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }
    
        protected override void SetDefaultFontColor()
        {
            if(!enableHighlight) return;
            
            OptionMenuItems.ForEach(value => value.Text.color = fontColour);
        }
        
        protected void UpdateVisibleItems()
        {
            var (_, cursorPosition) = CurrentCursorPosition;
            for (var i = 0; i < OptionMenuItems.Count; i++)
            {
                var listIndex = CurrentListPosition - cursorPosition + i;
                OptionMenuItems[i].SetMenuItem(listIndex >= OptionsList.Count ? default : OptionsList[listIndex]);
            }
        }
    }
}