using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public interface IMenuItem<out T>
    {
        public abstract T Value { get; }
        public Transform Transform  { get; }
        public Text Text  { get; }
        
        public string ToString();
        public bool IsNullOrEmpty();
    }
}