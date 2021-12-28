using System;
using Menu;
using MyBox;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;

namespace ActionMenu
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