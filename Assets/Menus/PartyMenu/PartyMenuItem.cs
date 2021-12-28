using System;
using System.Collections.Generic;
using Battle;
using Menus.PartyMenu;
using MyBox;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [Serializable]
    public class PartyMenuItem : MonoBehaviour, IMenuItem<Pokemon>
    {
        [SerializeField] private PartyMenuDetails pokemonDetails;
        [SerializeField] private PartyMenuItemPlate backplate;
        
        public Pokemon Value { get; private set; }
        public Transform Transform => transform;
        public Text Text => null;

        public void SetMenuItem(Pokemon pokemon)
        {
            Value = pokemon;

            if (Value == null) {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                pokemonDetails.SetData(Value); 
                backplate.SetNotSelected(Value.IsFainted);
            }
        }

        public void SetSelected() => backplate.SetSelected(Value.IsFainted);
        public void SetNotSelected() => backplate.SetNotSelected(Value.IsFainted);
        
        public override string ToString()
        {
            return Value.Name;
        }

        public bool IsNotNullOrEmpty() => Value != null;
    }
}