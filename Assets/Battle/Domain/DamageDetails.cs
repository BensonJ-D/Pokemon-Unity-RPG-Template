using System;
using Characters.Monsters;
using Characters.Moves;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle.Domain
{
    public readonly struct DamageDetails
    {
        public readonly bool Critical;
        public readonly AttackEffectiveness Effective;
        public readonly bool Fainted;
        public readonly int DamageDealt;
        public readonly float Multiplier;

        public DamageDetails(bool critical, AttackEffectiveness effective, bool fainted, int damageDealt,
            float multiplier)
        {
            Critical = critical;
            Effective = effective;
            Fainted = fainted;
            DamageDealt = damageDealt;
            Multiplier = multiplier;
        }

        public DamageDetails(bool fainted, int damageDealt)
        {
            Critical = false;
            Effective = AttackEffectiveness.NormallyEffective;
            Fainted = fainted;
            DamageDealt = damageDealt;
            Multiplier = 1f;
        }

        public static DamageDetails CalculateDamage(Pokemon attacker, Pokemon defender, Move move)
        {
            var critical = Random.value <= 0.0625f;
            var effectivenessMultiplier = MoveBase.TypeChart[(move.Base.Type, defender.Base.Type1)] *
                                          MoveBase.TypeChart[(move.Base.Type, defender.Base.Type2)];
            var typeAdvantage = MoveBase.GetEffectiveness(effectivenessMultiplier);
    
            var criticalMultiplier = critical ? 2.0f : 1.0f;
            var variability = Random.Range(0.85f, 1f);
    
            var attack = 0;
            var defence = 0;
            switch (move.Base.Category)
            {
                case MoveCategory.Physical:
                    attack = attacker.BoostedAttack;
                    defence = defender.BoostedDefence;
                    break;
                case MoveCategory.Special:
                    attack = attacker.BoostedSpAttack;
                    defence = defender.BoostedSpDefence;
                    break;
                case MoveCategory.Status:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    
            var multiplier = effectivenessMultiplier * criticalMultiplier;
            var a = 2 * attacker.Level / 5;
            var b = a * move.Base.Power * attack / defence;
            var c = b / 50 + 2;
            var damage = move.Base.Category != MoveCategory.Status 
                ? Mathf.FloorToInt(c * variability * multiplier)
                : 0;
            var fainted = defender.CurrentHp <= damage;
            return new DamageDetails(critical, typeAdvantage, fainted, damage, multiplier);
        }
    }
}