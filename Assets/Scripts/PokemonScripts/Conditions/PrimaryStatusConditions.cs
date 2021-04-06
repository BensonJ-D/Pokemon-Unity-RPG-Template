using System.Collections.Generic;
using System.Collections.ObjectModel;
using PokemonScripts.Conditions;
using PokemonScripts.Moves;
using PokemonScripts.Moves.Effects;

namespace PokemonScripts
{
    public enum PrimaryStatusCondition { None, Poison, Burn, Paralyse, Freeze, Sleep }

    public static class PrimaryStatusConditions
    {
        public static readonly ReadOnlyDictionary<PrimaryStatusCondition, StatusCondition> GetEffectClass =
            new ReadOnlyDictionary<PrimaryStatusCondition, StatusCondition>(
                new Dictionary<PrimaryStatusCondition, StatusCondition>()
                {
                    { PrimaryStatusCondition.None, null },
                    { PrimaryStatusCondition.Poison, new StatusCondition()
                    {
                        OnAfterTurn = (Pokemon pokemon) =>
                        {
                            pokemon.CurrentHp -= pokemon.MaxHp / 8;
                            return true;
                        }
                    } }
                }
            );
    }
}