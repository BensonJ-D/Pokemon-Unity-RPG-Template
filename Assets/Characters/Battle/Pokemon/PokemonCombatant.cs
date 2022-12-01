using System;
using Battle.Controller;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Battle.Pokemon
{
    public class PokemonCombatant : Combatant
    {
        [SerializeField] private Image image;
        [SerializeField] private int team;
        [SerializeField] private int position;

        public int Team => team;
        public int Position => position;
        public Monsters.Pokemon Pokemon { get; private set; }
        public Player.Player ControllingPlayer { get; set; }

        public void Setup(Monsters.Pokemon pokemon)
        {
            pokemon.StatusUI = pokemonStatus;
            Pokemon = pokemon;
            image.sprite = displayFront ? Pokemon.Base.FrontSprite : Pokemon.Base.BackSprite;
        }
    }
}