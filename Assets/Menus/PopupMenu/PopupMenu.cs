using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Utlilities;
using System.Window;
using System.Window.Menu;
using System.Window.Menu.ScrollMenu;
using MyBox;
using UnityEngine;

namespace Menus.PopupMenu
{ 
    public class PopupMenu<T> : ScrollMenu<T> where T : Enum
    {
        [Separator("Popup Menu Settings")] 
        [SerializeField] private float width;
        [SerializeField] private float spacing;
        [SerializeField] private float border;
        [SerializeField] private Vector2 padding;

        public delegate IEnumerator OnConfirmFunc(T choice);
        private OnConfirmFunc _onConfirm;

        protected List<IMenuItem<T>> PopupMenuItems;

        public virtual void Start()
        {
            Initiate();
            
            OptionMenuItems = new List<IMenuItem<T>>();
            PopupMenuItems.ForEach(option => OptionMenuItems.Add(option));
        }

        public IEnumerator ShowWindow(List<T> options, OnConfirmFunc onConfirmCallback)
        {
            _onConfirm = onConfirmCallback;
            
            SetMenu(options);
            SetWindowSize();
            yield return base.OpenWindow();
        }

        protected override IEnumerator OnConfirm() => _onConfirm(CurrentOption.Value);
        protected override IEnumerator OnCancel() => CloseWindow();

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