using System;
using System.Window.Menu;
using Characters.Monsters;
using Characters.UI;
using GameSystem.Window.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Summary
{
    [Serializable]
    public class SummaryStatsMenuItem : MonoBehaviour, IMenuItem<Pokemon>
    {
        [SerializeField] private Image pokemonSprite;
        [SerializeField] private CharacterStatus pokemonStatus;

        public Pokemon Value { get; set; }

        public CharacterStatus PokemonStatus => pokemonStatus;

        public Transform Transform => transform;
        public TextMeshProUGUI Text => null;

        public void SetMenuItem(Pokemon pokemon)
        {
            pokemon.StatusUI = pokemonStatus;
            pokemonSprite.sprite = pokemon.Base.FrontSprite;

            Value = pokemon;
        }

        public override string ToString() => Value.ToString();
        public bool IsNotNullOrEmpty() => Value != null;
    }
}