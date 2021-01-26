using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PokemonScripts
{
    [Serializable]
    public class Pokemon
    {
        [SerializeField] private PokemonBase _base;
        [SerializeField] private int _level;
        
        public string Name { get; private set; }

        public PokemonBase Base
        {
            get => _base;
            private set => _base = value;
        }

        public int Level
        {
            get => _level;
            private set => _level = value;
        }

        public int Hp { get; set; }
        public List<Move> Moves { get; private set; }

        public void Initialization()
        {
            Initialization(_base, _level);
        }
        
        public void Initialization(PokemonBase @base, int level)
        {
            _base = @base;
            _level = level;
            Name = _base.Species;
            
            Hp = MaxHp;

            Moves = new List<Move>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level > Level) break;

                if (Moves.Exists(oldMove => move.Base.Number == oldMove.Base.Number)) continue;

                Move newMove = new Move(move.Base);
                Moves.Add(newMove);

                if (Moves.Count > 4) Moves.RemoveAt(0);
            }
        }

        public DamageDetails TakeDamage(Move move, Pokemon attacker)
        {
            var critical = (Random.value <= 0.0625f);
            var effectivenessMultiplier = MoveBase.TypeChart[(move.Base.Type, Base.Type1)] *
                                          MoveBase.TypeChart[(move.Base.Type, Base.Type2)];
            var typeAdvantage = MoveBase.GetEffectiveness(effectivenessMultiplier);

            var criticalMultiplier = critical ? 2.0f : 1.0f;
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

            var multiplier = effectivenessMultiplier * criticalMultiplier;
            var a = (2 * attacker.Level + 10) / 250f;
            var d = a * move.Base.Power * atkVsDef + 2;
            var damage = Mathf.FloorToInt(d * variability * multiplier);
            var fainted = Hp <= damage;

            Hp = fainted ? 0 : Hp - damage;

            return new DamageDetails(critical, typeAdvantage, fainted, damage, multiplier);
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
        }
    }
}