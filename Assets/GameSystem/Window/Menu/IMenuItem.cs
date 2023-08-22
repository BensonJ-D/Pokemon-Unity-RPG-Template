using TMPro;
using UnityEngine;

namespace GameSystem.Window.Menu
{
    public interface IMenuItem<T>
    {
        public T Value { get; }
        public Transform Transform  { get; }
        public TextMeshProUGUI Text  { get; }

        public void SetMenuItem(T itemData);
        
        public string ToString();
        public bool IsNotNullOrEmpty();
    }
}