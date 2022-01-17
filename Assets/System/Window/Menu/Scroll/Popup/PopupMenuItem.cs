using System.Utlilities;
using UnityEngine;

namespace System.Window.Menu.Scroll.Popup
{
    [Serializable]
    public class PopupMenuItem<T> : MonoBehaviour, IMenuItem<T> where T : Enum
    {
        [SerializeField] private UnityEngine.UI.Text label;

        public T Value { get; protected set; }
        public Transform Transform => transform;
        public UnityEngine.UI.Text Text => label;
        
        public virtual void SetMenuItem(T option)
        {
            Value = option;
            
            transform.gameObject.SetActive(IsNotNullOrEmpty());
            Text.text = Value.GetDescription();
        }
        
        public override string ToString() => Value.GetDescription();
        public bool IsNotNullOrEmpty() => Value.IsNotDefault();
    }
}