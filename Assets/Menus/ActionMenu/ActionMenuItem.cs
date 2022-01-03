using System;
using System.Window.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.ActionMenu
{
    [Serializable]
    public class ActionMenuItem : MonoBehaviour, IMenuItem<ActionMenuOption>
    {
        [SerializeField] private Text text;
        [SerializeField] private ActionMenuOption action;

        public ActionMenuOption Value => action;

        public Transform Transform => transform;
        public Text Text => text;

        public void SetMenuItem(ActionMenuOption option) => throw new NotImplementedException();

        public override string ToString() => Value.ToString();
        public bool IsNotNullOrEmpty() => true;
    }
}