using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pokemon
{
    public class Pokemon
    {
        public PokemonBase Base { get; }

        public int Level { get; }

        public int Hp { get; set; }
        public List<Move> Moves { get; }

        public Pokemon(PokemonBase pBase, int pLevel)
        {
            Base = pBase;
            Level = pLevel;
            Hp = MaxHp;
        
            Moves = new List<Move>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level > Level) break;
        
                Move newMove = new Move(move.Base);
                Moves.Add(newMove);
        
                if (Moves.Count > 4) Moves.RemoveAt(0);
            }
        }

        public DamageDetails TakeDamage(Move move, Pokemon attacker)
        {
            var critical = (Random.value <= 0.0625f);
            var effectiveness = MoveBase.TypeChart[(move.Base.Type, Base.Type1)] * MoveBase.TypeChart[(move.Base.Type, Base.Type2)];
            var typeAdvantage = MoveBase.GetEffectiveness(effectiveness);

            var criticalModifier = critical ? 2.0f : 1.0f;
            var variability = Random.Range(0.85f, 1f);
        
            var atkVsDef = 0f;
            switch (move.Base.DamageType)
            {
                case DamageType.Physical:
                    atkVsDef = (float) attacker.Attack / attacker.Defence;
                    break;
                case DamageType.Special:
                    atkVsDef = (float) attacker.SpAttack / attacker.SpDefence;
                    break;
                case DamageType.Status:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var a = (2 * attacker.Level + 10) / 250f;
            var d = a * move.Base.Power * atkVsDef + 2;
            var damage = Mathf.FloorToInt(d * variability * effectiveness * criticalModifier);
            var fainted = Hp < damage;

            Hp = fainted ? 0 : Hp - damage;
        
            return new DamageDetails(critical, typeAdvantage, fainted, damage);
        }
    
        public int MaxHp => Mathf.FloorToInt((2 * Base.MaxHp * Level) / 100f) + 10 + Level;
        public int Attack => Mathf.FloorToInt((2 * Base.Attack * Level) / 100f) + 5;
        public int Defence => Mathf.FloorToInt((2 * Base.Defence * Level) / 100f) + 5;
        public int SpAttack => Mathf.FloorToInt((2 * Base.SpAttack * Level) / 100f) + 5;
        public int SpDefence => Mathf.FloorToInt((2 * Base.SpDefence * Level) / 100f) + 5;
        public int Speed => Mathf.FloorToInt((2 * Base.Speed * Level) / 100f) + 5;

        public readonly struct DamageDetails
        {
            public readonly bool Critical;
            public readonly AttackEffectiveness Effective;
            public readonly bool Fainted;
            public readonly int DamageDealt;

            public DamageDetails(bool critical, AttackEffectiveness effective, bool fainted, int damageDealt)
            {
                Critical = critical;
                Effective = effective;
                Fainted = fainted;
                DamageDealt = damageDealt;
            }
        }
    }
}