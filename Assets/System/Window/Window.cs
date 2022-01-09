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

        protected bool Initialised { get; private set; }
        protected bool WindowOpen { get; private set; }
        private Vector3 DefaultPosition { get; set; }
        
        private Vector3 CanvasOrigin { get; set; }

        protected virtual void Initialise()
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

        protected IEnumerator OpenWindow(Vector2 pos = default)
        {
            if (pos == default) pos = DefaultPosition;
            
            yield return BeforeOpen();
            
            transform.position = pos;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            yield return null;
            canvas.enabled = true;
            WindowOpen = true;
            
            yield return OnOpen();
        }

        public virtual IEnumerator CloseWindow()
        {
            yield return BeforeClose();
            
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = CanvasOrigin;
            yield return null;
            canvas.enabled = false;
            WindowOpen = false;

            yield return OnClose();
        }

        protected virtual IEnumerator BeforeOpen() => null;
        protected virtual IEnumerator OnOpen() => null;
        
        protected virtual IEnumerator BeforeClose() => null;
        protected virtual IEnumerator OnClose() => null;
    }
}
