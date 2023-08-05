using System;
using System.Collections;
using Battle.Controller;
using Battle.Domain;
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

        public IEnumerator ApplyDamage(DamageDetails damageDetails)
        {
            var decayMultiplier = 100U;
            decayMultiplier += (uint) (damageDetails.Multiplier * 25U);
            decayMultiplier += damageDetails.Critical ? 50U : 0U;
            yield return Pokemon.UpdateHealth(-damageDetails.DamageDealt, decayMultiplier);
        }
    }
}