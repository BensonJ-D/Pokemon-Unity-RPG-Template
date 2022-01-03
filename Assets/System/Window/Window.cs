using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using VFX;

namespace System.Window
{
    public abstract class Window : MonoBehaviour
    {
        [Separator("Window Settings")]
        // [SerializeField] protected Vector2 size;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Image background;

        public WindowCloseReason CloseReason { get; protected set; }
        
        protected bool WindowOpen { get; private set; }
        protected Vector3 DefaultPosition { get; private set; }
        protected bool IsCloseable { get; private set; }
        
        private Vector3 CanvasOrigin { get; set; }

        protected void Initiate()
        {
            DefaultPosition = transform.position;
            CanvasOrigin = canvas.transform.position;
            canvas.enabled = false;
        }

        protected void SetSize(float width, float height)
        {
            if (background == null) return;
            
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            background.rectTransform.ForceUpdateRectTransforms();
        }

        public virtual IEnumerator ShowWindow(bool isCloseable = true) => ShowWindow(DefaultPosition, isCloseable);

        protected virtual IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            transform.position = pos;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            yield return null;
            canvas.enabled = true;
            WindowOpen = true;
            
            IsCloseable = isCloseable;

            yield return TransitionController.Instance.WaitForTransitionCompletion();
            OnOpen();
        }

        public virtual IEnumerator CloseWindow(WindowCloseReason closeReason) => HideWindow(closeReason);
        
        protected virtual IEnumerator HideWindow(WindowCloseReason closeReason)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = CanvasOrigin;
            yield return null;
            canvas.enabled = false;
            WindowOpen = false;

            CloseReason = closeReason;
            OnClose(closeReason);
        }
        
        protected virtual IEnumerator OnConfirm() { return HideWindow(WindowCloseReason.Complete); }
        protected virtual IEnumerator OnCancel() { return HideWindow(WindowCloseReason.Cancel); }

        protected virtual void OnOpen() { }
        protected virtual void OnClose(WindowCloseReason closeReason) { }
    }
}
