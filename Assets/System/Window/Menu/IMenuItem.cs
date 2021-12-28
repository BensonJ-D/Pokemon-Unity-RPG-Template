using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public interface IMenuItem<T>
    {
        public T Value { get; }
        public Transform Transform  { get; }
        public Text Text  { get; }

        public void SetMenuItem(T itemData);
        
        public string ToString();
        public bool IsNotNullOrEmpty();
    }
}