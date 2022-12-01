using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace System.Window
{
    public abstract class WindowBase : MonoBehaviour
    {
        [Separator("Window Settings")]
        // [SerializeField] protected Vector2 size;
        [SerializeField] protected bool isChildWindow;
        [ConditionalField(nameof(isChildWindow), true)] [SerializeField] protected Canvas canvas;
        [SerializeField] protected Image background;

        protected bool Initialised { get; private set; }
        protected bool WindowOpen { get; private set; }
        private Vector3 DefaultPosition { get; set; }
        
        private Vector3 CanvasOrigin { get; set; }

        public virtual void Initialise()
        {
            Initialised = true;
            DefaultPosition = transform.localPosition;
            SetVisibility(false);
            if(!isChildWindow) CanvasOrigin = canvas.transform.position;
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
            
            transform.localPosition = pos;
            if(!isChildWindow) canvas.renderMode = RenderMode.ScreenSpaceCamera;
            yield return null;
            SetVisibility(true);
            WindowOpen = true;
            
            yield return OnOpen();
        }

        public virtual IEnumerator CloseWindow()
        {
            yield return BeforeClose();

            if (!isChildWindow)
            {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.position = CanvasOrigin;
            }

            yield return null;
            SetVisibility(false);
            WindowOpen = false;

            yield return OnClose();
        }

        protected virtual IEnumerator BeforeOpen() => null;
        protected virtual IEnumerator OnOpen() => null;
        
        protected virtual IEnumerator BeforeClose() => null;
        protected virtual IEnumerator OnClose() => null;

        private void SetVisibility(bool visible)
        {
            if (isChildWindow)
            {
                gameObject.SetActive(visible);
            }
            else
            {
                canvas.enabled = visible;
            }
        }
    }
}
