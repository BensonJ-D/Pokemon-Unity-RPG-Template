using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle.Domain
{
    public readonly struct DamageDetails
    {
        public readonly PokemonCombatant Attacker;
        public readonly PokemonCombatant Target;
        public readonly bool Critical;
        public readonly AttackEffectiveness Effective;
        public readonly bool Fainted;
        public readonly int DamageDealt;
        public readonly float Multiplier;

        public DamageDetails(PokemonCombatant attacker, PokemonCombatant target, bool critical, 
            AttackEffectiveness effective, bool fainted, int damageDealt, float multiplier)
        {
            Attacker = attacker;
            Target = target;
            Critical = critical;
            Effective = effective;
            Fainted = fainted;
            DamageDealt = damageDealt;
            Multiplier = multiplier;
        }

        public static DamageDetails CalculateDamage(PokemonCombatant attacker, PokemonCombatant target, Move move)
        {
            var atkPokemon = attacker.Pokemon;
            var defPokemon = target.Pokemon;
            var critical = Random.value <= 0.0625f;
            
            var sameTypeMultiplier = 
                move.Base.Type == atkPokemon.Base.Type1 || move.Base.Type == atkPokemon.Base.Type2 ? 1.5f : 1f;
            
            var effectivenessMultiplier = MoveBase.TypeChart[(move.Base.Type, defPokemon.Base.Type1)] *
                                          MoveBase.TypeChart[(move.Base.Type, defPokemon.Base.Type2)];
            
            var typeAdvantage = MoveBase.GetEffectiveness(effectivenessMultiplier);
    
            var criticalMultiplier = critical ? 1.5f : 1.0f;
            var variability = Random.Range(85, 101);
    
            var attack = 0;
            var defence = 0;
            switch (move.Base.Category)
            {
                case MoveCategory.Physical:
                    attack = atkPokemon.BoostedAttack;
                    defence = defPokemon.BoostedDefence;
                    break;
                case MoveCategory.Special:
                    attack = atkPokemon.BoostedSpAttack;
                    defence = defPokemon.BoostedSpDefence;
                    break;
                case MoveCategory.Status:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    
            var multiplier = effectivenessMultiplier * criticalMultiplier;
            var a = 2 * atkPokemon.Level / 5 + 2;
            var b = a * move.Base.Power * attack / defence;
            var damage = b / 50 + 2;
            
            damage = (int)(damage * criticalMultiplier);
            damage = damage * variability / 100;
            damage = (int)(damage * sameTypeMultiplier);
            damage = (int)(damage * effectivenessMultiplier);
            
            Debug.Log($"damage dealt {damage}");
            var fainted = defPokemon.CurrentHp <= damage;
            return new DamageDetails(attacker, target, critical, typeAdvantage, fainted, damage, multiplier);
        }

        public static List<DamageDetails> CalculateDamage(PokemonCombatant attacker,
            IEnumerable<PokemonCombatant> targets, Move move) =>
            targets.Select(target => CalculateDamage(attacker, target, move))
                .ToList();
    }
}