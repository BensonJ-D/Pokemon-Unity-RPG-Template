using MyBox;
using UnityEngine;

namespace System.Window.Menu
{
    [Serializable]
    public abstract class Menu<T> : Window
    {
        [Separator("Menu Settings")]
        [SerializeField] protected bool enableHighlight;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] protected Color highlightColour;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] protected Color fontColour;
    
        [SerializeField] protected bool enableCursor;
        [ConditionalField(nameof(enableCursor))] [SerializeField] protected MenuCursor cursor;
        
        public (int, int) CurrentCursorPosition { get; protected set; }
        public IMenuItem<T> CurrentOption { get; protected set; }

        public IMenuItem<T> Choice { get; protected set; }

        protected void SetCursorPosition(int x, int y)
        {
            if (!enableCursor) return;
        
            var cursorPos = CurrentOption.Transform.localPosition;
            cursor.SetPosition(cursorPos.x, cursorPos.y);
        }

        protected abstract void SetDefaultFontColor();
    
        protected void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next)
        {
            if(!enableHighlight) return;
        
            if(prev != null) prev.Text.color = fontColour;
            if(next != null) next.Text.color = highlightColour;
        }
    }
}