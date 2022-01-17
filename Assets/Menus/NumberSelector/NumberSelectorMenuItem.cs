using System.Window.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.NumberSelector
{
    public class NumberSelectorMenuItem : MonoBehaviour, IMenuItem<int>
    {
        [SerializeField] private Text label;

        public int Value { get; protected set; }
        public Transform Transform => transform;
        public Text Text => label;
        
        public void SetMenuItem(int option)
        {
            Value = option;
            
            transform.gameObject.SetActive(IsNotNullOrEmpty());
            Text.text = ToString();
        }
        
        public override string ToString() => Value.ToString().PadLeft(3, '0');
        public bool IsNotNullOrEmpty() => true;
    }
}