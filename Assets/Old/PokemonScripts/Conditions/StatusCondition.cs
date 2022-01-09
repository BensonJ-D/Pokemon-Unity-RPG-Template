using System.Collections;
using Battle;

namespace PokemonScripts.Conditions
{
    public abstract class StatusCondition
    {
        public virtual IEnumerator OnBeforeAction(BattlePokemon battlePokemon, BattleDialogBox battleDialogBox) { yield break; }

        public virtual IEnumerator OnAfterTurn(BattlePokemon battlePokemon, BattleDialogBox battleDialogBox) { yield break; }
    }
}