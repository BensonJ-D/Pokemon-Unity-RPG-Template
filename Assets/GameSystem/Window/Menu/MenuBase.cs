using System.Collections;
using GameSystem.Window.Menu;
using MyBox;
using UnityEngine;

namespace System.Window.Menu
{
    [Serializable]
    public abstract class MenuBase<T> : WindowBase
    {
        public delegate IEnumerator OnCancelFunc();

        public delegate IEnumerator OnConfirmFunc(T choice);

        [Separator("Menu Settings")] [SerializeField]
        protected bool enableHighlight;

        [ConditionalField(nameof(enableHighlight))] [SerializeField]
        protected Color highlightColour;

        [ConditionalField(nameof(enableHighlight))] [SerializeField]
        protected Color fontColour;

        [SerializeField] protected bool enableCursor;

        [ConditionalField(nameof(enableCursor))] [SerializeField]
        protected MenuCursor cursor;

        protected OnCancelFunc OnCancelCallback;
        protected OnConfirmFunc OnConfirmCallback;


        public WindowCloseReason? CloseReason { get; protected set; }
        public (int, int) CurrentCursorPosition { get; protected set; }
        public IMenuItem<T> CurrentOption { get; protected set; }
        public IMenuItem<T> Choice { get; protected set; }

        public void Reset() {
            Choice = null;
            CloseReason = null;
        }

        public virtual IEnumerator OpenWindow(Vector2 pos = default, OnConfirmFunc onConfirmCallback = null,
            OnCancelFunc onCancelCallback = null) {
            Choice = null;
            CloseReason = null;

            OnConfirmCallback = onConfirmCallback;
            OnCancelCallback = onCancelCallback;

            yield return base.OpenWindow(pos);
        }

        protected virtual void SetCursorPosition(int x, int y) {
            if (!enableCursor) return;
            cursor.SetPosition(x, y);
        }

        protected abstract void SetDefaultFontColor();

        protected virtual void SetNewHighlightedOption(IMenuItem<T> prev, IMenuItem<T> next) {
            if (!enableHighlight) return;

            if (prev != null) prev.Text.color = fontColour;
            if (next != null) next.Text.color = highlightColour;
        }

        protected virtual IEnumerator OnConfirm() {
            return OnConfirmCallback?.Invoke(CurrentOption.Value);
        }

        protected virtual IEnumerator OnCancel() {
            return OnCancelCallback?.Invoke();
        }
    }
}