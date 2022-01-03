using UnityEngine;

namespace System.Window.Menu
{
    public interface IMenuItem<T>
    {
        public T Value { get; }
        public Transform Transform  { get; }
        public UnityEngine.UI.Text Text  { get; }

        public void SetMenuItem(T itemData);
        
        public string ToString();
        public bool IsNotNullOrEmpty();
    }
}