using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public abstract class Window : MonoBehaviour
    {
        // [SerializeField] protected Vector2 size;
        [SerializeField] protected Canvas canvas;
        [SerializeField] protected Image background;

        public WindowCloseReason CloseReason { get; private set; }
        
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

        public virtual IEnumerator ShowWindow(bool isCloseable = true)
        {
            yield return ShowWindow(DefaultPosition, isCloseable);
        }

        protected virtual IEnumerator ShowWindow(Vector2 pos, bool isCloseable = true)
        {
            transform.position = pos;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            yield return null;
            canvas.enabled = true;
            WindowOpen = true;
            
            IsCloseable = isCloseable;
            OnOpen();
        }
    
        protected void HideWindow(WindowCloseReason closeReason)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = CanvasOrigin;
            canvas.enabled = false;
            WindowOpen = false;

            CloseReason = closeReason;
            OnClose(closeReason);
        }
    
    
        protected virtual void OnConfirm() { HideWindow(WindowCloseReason.Complete); }
        protected virtual void OnCancel() { HideWindow(WindowCloseReason.Cancel); }

        protected virtual void OnOpen() { }
        protected virtual void OnClose(WindowCloseReason closeReason) { }
    }
}
