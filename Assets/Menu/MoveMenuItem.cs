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
        private Move _move;
        
        public Move Value => _move;
        public Transform Transform => transform;
        public Text Text => text;

        public void SetMove(Move move)
        {
            _move = move;
            Text.text = move == null ? "-" : move.Base.Name;
        }

        public override string ToString()
        {
            return Value.DebugString();
        }

        public bool IsNullOrEmpty() => _move == null;
    }
}