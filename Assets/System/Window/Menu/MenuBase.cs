using System.Collections;
using MyBox;
using UnityEngine;

namespace System.Window.Menu
{
    [Serializable]
    public abstract class MenuBase<T> : WindowBase
    {
        [Separator("Menu Settings")]
        [SerializeField] protected bool enableHighlight;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] protected Color highlightColour;
        [ConditionalField(nameof(enableHighlight))] [SerializeField] protected Color fontColour;
    
        [SerializeField] protected bool enableCursor;
        [ConditionalField(nameof(enableCursor))] [SerializeField] protected MenuCursor cursor;
        
        public WindowCloseReason CloseReason { get; protected set; }
        public (int, int) CurrentCursorPosition { get; protected set; }
        public IMenuItem<T> CurrentOption { get; protected set; }
        public IMenuItem<T> Choice { get; protected set; }
        
        public delegate IEnumerator OnConfirmFunc(T choice);
        public delegate IEnumerator OnCancelFunc();
        protected OnConfirmFunc _onConfirm;
        protected OnCancelFunc _onCancel;

        public virtual IEnumerator OpenWindow(Vector2 pos = default, OnConfirmFunc onConfirmCallback = null,
            OnCancelFunc onCancelCallback = null)
        {
            _onConfirm = onConfirmCallback;
            _onCancel = onCancelCallback;
            
            yield return base.OpenWindow(pos);
        }
        
        protected void SetCursorPosition(int x, int y)
        {
            if (!enableCursor) return;
            cursor.SetPosition(x, y);
        }

        protected abstract void SetDefaultFontColor();
        protected virtual  void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next)
        {
            if(!enableHighlight) return;
        
            if(prev != null) prev.Text.color = fontColour;
            if(next != null) next.Text.color = highlightColour;
        }
        
        protected virtual IEnumerator OnConfirm() => _onConfirm?.Invoke(CurrentOption.Value);
        protected virtual IEnumerator OnCancel() => _onCancel?.Invoke();
    }
}