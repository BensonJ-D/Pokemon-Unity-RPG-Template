using System;
using System.Window.Menu;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.StringGridMenu
{
    [Serializable]
    public class StringMenuItem : MonoBehaviour, IMenuItem<string>
    {
        [SerializeField] private Text text;

        public string Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => text;

        public void SetMenuItem(string option) => Value = option;

        public override string ToString() => Value;
        public bool IsNotNullOrEmpty() => !Value.IsNullOrEmpty();
    }
}