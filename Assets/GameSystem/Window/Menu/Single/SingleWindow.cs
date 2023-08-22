using System.Collections;
using UnityEngine;

namespace System.Window.Menu.Single
{
    [Serializable]
    public abstract class SingleWindow : WindowBase
    {
        public delegate IEnumerator OnConfirmFunc();
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
        
        protected virtual IEnumerator OnConfirm() => _onConfirm?.Invoke();
        protected virtual IEnumerator OnCancel() => _onCancel?.Invoke();
    }
}