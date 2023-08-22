using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Utlilities;
using GameSystem.Window.Menu;
using MyBox;
using UnityEngine;

namespace System.Window.Menu.Scroll.Popup
{ 
    public class PopupMenu<T> : ScrollMenu<T> where T : Enum
    {
        [Separator("Popup Menu Settings")] 
        [SerializeField] private float width;
        [SerializeField] private float spacing;
        [SerializeField] private float border;
        [SerializeField] private Vector2 padding;

        protected List<IMenuItem<T>> PopupMenuItems;

        public virtual void Start()
        {
            Initialise();
            
            OptionMenuItems = new List<IMenuItem<T>>();
            PopupMenuItems.ForEach(option => OptionMenuItems.Add(option));
        }

        public IEnumerator OpenWindow(List<T> options, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            SetMenu(options);
            SetWindowSize();
            yield return base.OpenWindow(onConfirmCallback: onConfirmCallback, onCancelCallback: onCancelCallback);
        }

        private void SetWindowSize()
        {
            SetSize(width, OptionsList.Count * spacing + border);

            var menuIndex = OptionsList.Count - 1;
            OptionMenuItems.ForEach(menuItem => menuItem.Transform.localPosition = new Vector3(padding.x, padding.y + spacing * menuIndex--));
        }

        private void SetMenu(List<T> options)
        {
            var nonNullOptions = options.Where(option => option.IsNotDefault()).ToList();
            OptionsList = nonNullOptions;
            
            using var enumMenuItems = PopupMenuItems.GetEnumerator();
            using var enumOptions = nonNullOptions.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumOptions.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumOptions.Current);
            }
        }
    }
}