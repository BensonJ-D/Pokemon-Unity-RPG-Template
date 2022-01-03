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
        
        protected List<IMenuItem<T>> PopupMenuItems;
        private bool _closeOnConfirm = true;

        public virtual void Start()
        {
            Initiate();
            
            OptionMenuItems = new List<IMenuItem<T>>();
            PopupMenuItems.ForEach(option => OptionMenuItems.Add(option));
        }

        public IEnumerator ShowWindow(List<T> options, bool closeOnConfirm = true)
        {
            _closeOnConfirm = closeOnConfirm;
            
            SetMenu(options);
            SetWindowSize();
            yield return base.ShowWindow();
        }

        protected override IEnumerator OnConfirm() => _closeOnConfirm ? base.OnConfirm() : OnConfirmNoClose();

        private IEnumerator OnConfirmNoClose()
        {
            CloseReason = WindowCloseReason.Complete;
            yield return null;
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