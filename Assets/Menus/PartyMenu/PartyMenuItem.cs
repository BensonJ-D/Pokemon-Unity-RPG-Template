using System;
using System.Window.Menu;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.PartyMenu
{
    [Serializable]
    public class PartyMenuItem : MonoBehaviour, IMenuItem<Pokemon>
    {
        [SerializeField] private Image pokemonIcon;
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
                pokemonIcon.sprite = Value.Base.Icon;
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