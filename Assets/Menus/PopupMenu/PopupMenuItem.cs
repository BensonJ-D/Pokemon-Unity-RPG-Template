using System;
using System.Utlilities;
using System.Window.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.PopupMenu
{
    [Serializable]
    public class PopupMenuItem<T> : MonoBehaviour, IMenuItem<T> where T : Enum
    {
        [SerializeField] private Text label;

        public T Value { get; protected set; }
        public Transform Transform => transform;
        public Text Text => label;
        
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