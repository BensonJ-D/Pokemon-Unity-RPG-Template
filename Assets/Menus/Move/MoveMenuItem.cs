using System;
using System.Window.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Move
{
    [Serializable]
    public class MoveMenuItem : MonoBehaviour, IMenuItem<PokemonScripts.Moves.Move>
    {
        [SerializeField] private Text text;
        
        public PokemonScripts.Moves.Move Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => text;

        public void SetMenuItem(PokemonScripts.Moves.Move move)
        {
            Value = move;
            Text.text = move == null ? "-" : move.Base.Name;
        }

        public override string ToString()
        {
            return Value.DebugString();
        }

        public bool IsNotNullOrEmpty() => Value != null;
    }
}