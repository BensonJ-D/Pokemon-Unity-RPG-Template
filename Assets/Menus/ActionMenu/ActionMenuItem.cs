using System;
using GameSystem.Window.Menu;
using TMPro;
using UnityEngine;

namespace Menus.ActionMenu
{
    [Serializable]
    public class ActionMenuItem : MonoBehaviour, IMenuItem<ActionMenuOption>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ActionMenuOption action;

        public ActionMenuOption Value => action;

        public Transform Transform => transform;
        public TextMeshProUGUI Text => text;

        public void SetMenuItem(ActionMenuOption option) => throw new NotImplementedException();

        public override string ToString() => Value.ToString();
        public bool IsNotNullOrEmpty() => true;
    }
}