using System.Collections;
using Characters.Monsters;
using GameSystem.Window.Menu;
using GameSystem.Window.Menu.Grid;
using MyBox;
using UnityEngine;

namespace Menus.Summary
{
    public class SummaryMenu : GridMenu<Pokemon>
    {
        [Separator("Summary UI")] [SerializeField]
        private SummaryStatsMenuItem statsSummaryView;

        public override void Initialise() {
            base.Initialise();

            OptionsGrid = new IMenuItem<Pokemon>[,] {
                {statsSummaryView}
            };
        }

        public IEnumerator OpenWindow(Pokemon pokemon, OnConfirmFunc onConfirmCallback = null,
            OnCancelFunc onCancelCallback = null) {
            SetPokemon(pokemon);
            yield return base.OpenWindow(onConfirmCallback: BoostExp, onCancelCallback: onCancelCallback);
        }

        private void SetPokemon(Pokemon pokemon) {
            statsSummaryView.SetMenuItem(pokemon);
            pokemon.StatusUI = statsSummaryView.PokemonStatus;
        }

        private IEnumerator BoostExp(Pokemon pokemon) {
            return pokemon.UpdateExp(5000);
        }
    }
}