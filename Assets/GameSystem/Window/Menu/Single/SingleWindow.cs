using System.Collections;
using UnityEngine;

namespace System.Window.Menu.Single
{
    [Serializable]
    public abstract class SingleWindow : WindowBase
    {
        public delegate IEnumerator OnCancelFunc();

        public delegate IEnumerator OnConfirmFunc();

        protected OnCancelFunc _onCancel;
        protected OnConfirmFunc _onConfirm;

        public virtual IEnumerator OpenWindow(Vector2 pos = default, OnConfirmFunc onConfirmCallback = null,
            OnCancelFunc onCancelCallback = null) {
            _onConfirm = onConfirmCallback;
            _onCancel = onCancelCallback;

            yield return base.OpenWindow(pos);
        }

        protected virtual IEnumerator OnConfirm() {
            return _onConfirm?.Invoke();
        }

        protected virtual IEnumerator OnCancel() {
            return _onCancel?.Invoke();
        }
    }
}