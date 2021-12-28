using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Menu
{ 
    public class PartyMenu : GridMenu<Pokemon>
    {
        [Separator("Move UI")]
        [SerializeField] private List<PartyMenuItem> menuItems;
        [SerializeField] private PokemonParty examplePokemon;

        public void Start()
        {
            Initiate();
            
            OptionsGrid = new IMenuItem<Pokemon>[,]
            {
                {menuItems[0], null, null, null, null, null},
                {null, menuItems[1], menuItems[2], menuItems[3], menuItems[4], menuItems[5]}
            };

            StartCoroutine(ShowWindow(examplePokemon));
        }

        public IEnumerator ShowWindow(PokemonParty pokemon)
        {
            SetParty(pokemon.Party);
            yield return base.ShowWindow();
        }

        protected override void OnOptionChange(IMenuItem<Pokemon> previousOption, IMenuItem<Pokemon> newOption)
        {
            base.OnOptionChange(previousOption, newOption);
            ((PartyMenuItem)previousOption)?.SetNotSelected();
            ((PartyMenuItem)newOption).SetSelected();
        }

        private void SetParty(List<Pokemon> pokemon)
        {
            using var enumMenuItems = menuItems.GetEnumerator();
            using var enumPokemon = pokemon.GetEnumerator();
            while (enumMenuItems.MoveNext())
            {
                enumPokemon.MoveNext();
                // ReSharper disable once PossibleNullReferenceException
                enumMenuItems.Current.SetMenuItem(enumPokemon.Current);
            }
        }
    }
}