using System;
using System.Collections.Generic;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [Serializable]
    public class MoveMenuItem : MonoBehaviour, IMenuItem<Move>
    {
        [SerializeField] private Text text;
        
        public Move Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => text;

        public void SetMenuItem(Move move)
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