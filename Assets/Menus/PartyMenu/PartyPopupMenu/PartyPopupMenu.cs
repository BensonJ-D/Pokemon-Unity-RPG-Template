using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionMenu;
using MyBox;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menu
{ 
    public class PartyPopupMenu : PopupMenu<PartyPopupMenuOption>
    {
        [Separator("Move UI")] 
        [SerializeField] private List<PartyPopupMenuItem> menuItems;
        [SerializeField] private List<PartyPopupMenuOption> exampleItems;
        
        public override void Start()
        {
            PopupMenuItems = new List<IMenuItem<PartyPopupMenuOption>>();
            menuItems.ForEach(menuItem => PopupMenuItems.Add(menuItem));
            
            base.Start();
            
            StartCoroutine(ShowWindow(exampleItems));
        }
    }
}