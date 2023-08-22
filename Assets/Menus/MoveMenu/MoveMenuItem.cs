using System;
using System.Window.Menu;
using GameSystem.Window.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MoveMenu
{
    [Serializable]
    public class MoveMenuItem : MonoBehaviour, IMenuItem<Characters.Moves.Move>
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public Characters.Moves.Move Value { get; private set; }
        public Transform Transform => transform;
        public TextMeshProUGUI Text => text;

        public void SetMenuItem(Characters.Moves.Move move)
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