using System;
using System.Window.Menu;
using Battle;
using Menus.ActionMenu;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.SummaryMenu
{
    [Serializable]
    public class SummaryStatsMenuItem : MonoBehaviour, IMenuItem<Pokemon>
    {
        [SerializeField] private Image pokemonSprite;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private Text attackText;
        [SerializeField] private Text defenceText;
        [SerializeField] private Text spAtkText;
        [SerializeField] private Text spDefText;
        [SerializeField] private Text speedText;
        [SerializeField] private ExperienceBar expBar;

        public Pokemon Value { get; set; }
    
        public Transform Transform => transform;
        public Text Text => null;

        public void SetMenuItem(Pokemon pokemon) {
            pokemonSprite.sprite = pokemon.Base.FrontSprite;
            healthBar.Setup(pokemon);
            attackText.text = pokemon.Attack().ToString();
            defenceText.text = pokemon.Defence().ToString();
            spAtkText.text = pokemon.SpAttack().ToString();
            spDefText.text = pokemon.SpDefence().ToString();
            speedText.text = pokemon.Speed().ToString();
            expBar.Setup(pokemon);

            Value = pokemon;
        }

        public override string ToString() => Value.ToString();
        public bool IsNotNullOrEmpty() => Value != null;
    }
}