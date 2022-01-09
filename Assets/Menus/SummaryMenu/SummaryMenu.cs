using System.Collections;
using System.Collections.Generic;
using System.Window.Menu;
using System.Window.Menu.GridMenu;
using Menus.ActionMenu;
using MyBox;
using PokemonScripts;
using UnityEditor;
using UnityEngine;

namespace Menus.SummaryMenu
{ 
    public class SummaryMenu : GridMenu<Pokemon>
    {
        [Separator("Summary UI")]
        [SerializeField] private SummaryStatsMenuItem statsSummaryView;
        
        public void Start()
        {
            Initialise();
            
            OptionsGrid = new IMenuItem<Pokemon>[,]
            {
                { statsSummaryView }
            };
        }
        
        public IEnumerator OpenWindow(Pokemon pokemon, OnConfirmFunc onConfirmCallback = null, OnCancelFunc onCancelCallback = null)
        {
            SetPokemon(pokemon);
            yield return base.OpenWindow(onConfirmCallback: onConfirmCallback, onCancelCallback: onCancelCallback);
        }

        private void SetPokemon(Pokemon pokemon)
        {
            statsSummaryView.SetMenuItem(pokemon);
        }
    }
}