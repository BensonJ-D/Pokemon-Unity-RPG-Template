using System;

namespace PokemonScripts.Conditions
{
    public class StatusCondition
    {
        public Action<Pokemon> OnBeforeAction;
        public Func<Pokemon, bool> OnAfterTurn;
    }
}