using System.Window.Menu;
using GameSystem.Window.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.NumberSelector
{
    public class NumberSelectorMenuItem : MonoBehaviour, IMenuItem<int>
    {
        [SerializeField] private TextMeshProUGUI label;

        public int Value { get; protected set; }
        public Transform Transform => transform;
        public TextMeshProUGUI Text => label;
        
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