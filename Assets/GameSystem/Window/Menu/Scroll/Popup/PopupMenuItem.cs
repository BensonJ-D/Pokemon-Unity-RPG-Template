using System;
using System.Utlilities;
using TMPro;
using UnityEngine;

namespace GameSystem.Window.Menu.Scroll.Popup
{
    [Serializable]
    public class PopupMenuItem<T> : MonoBehaviour, IMenuItem<T> where T : Enum
    {
        [SerializeField] private TextMeshProUGUI label;

        public T Value { get; protected set; }
        public Transform Transform => transform;
        public TextMeshProUGUI Text => label;
        
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