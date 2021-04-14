using System;
using System.Collections.Generic;
using PokemonScripts.Conditions;
using PokemonScripts.Moves;
using UnityEngine;

namespace PokemonScripts
{
    public enum Stat { HP, Attack, Defence, SpAttack, SpDefence, Speed, Accuracy, Evasion }

    [Serializable]
    public class Pokemon
    {
        [SerializeField] private PokemonBase _base;
        [SerializeField] private int level;

        public void Initialization()
        {
            Initialization(_base, level);
        }

        public void Initialization(PokemonBase @base, int level)
        {
            _base = @base;
            Level = level;
            Name = _base.Species;

            CurrentHp = MaxHp;
            Experience = ExperienceGroups.GetExperienceList[Base.ExperienceGroup][level - 1];

            Moves = new List<Move>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level > Level) break;

                if (Moves.Exists(oldMove => move.Base.Number == oldMove.Base.Number)) continue;

                Move newMove = new Move(move.Base);
                Moves.Add(newMove);

                if (Moves.Count > 4) Moves.RemoveAt(0);
            }

            StatBoosts = new Dictionary<Stat, int>
            {
                {Stat.Attack, 0},
                {Stat.Defence, 0},
                {Stat.SpAttack, 0},
                {Stat.SpDefence, 0},
                {Stat.Speed, 0}
            };
        }

        public string Name { get; private set; }

        public PokemonBase Base
        {
            get => _base;
            private set => _base = value;
        }

        public int Level { get; private set; }
        public int Experience { get; private set; }

        public PrimaryStatusCondition PrimaryCondition { get; private set; } = PrimaryStatusCondition.None;
        public List<SecondaryStatusCondition> SecondaryConditions { get; private set; } = new List<SecondaryStatusCondition>();

        public int MaxHp => Mathf.FloorToInt((2 * Base.MaxHp * Level) / 100f) + 10 + Level;
        public int Attack => Mathf.FloorToInt((2 * Base.Attack * Level) / 100f) + 5;
        public int Defence => Mathf.FloorToInt((2 * Base.Defence * Level) / 100f) + 5;
        public int SpAttack => Mathf.FloorToInt((2 * Base.SpAttack * Level) / 100f) + 5;
        public int SpDefence => Mathf.FloorToInt((2 * Base.SpDefence * Level) / 100f) + 5;
        public int Speed => Mathf.FloorToInt((2 * Base.Speed * Level) / 100f) + 5;
        
        public List<Move> Moves { get; private set; }

        public int CurrentHp { get; set; }
        public int BoostedAttack => GetBoostedStat(Stat.Attack, Attack);
        public int BoostedDefence => GetBoostedStat(Stat.Defence, Defence);
        public int BoostedSpAttack => GetBoostedStat(Stat.SpAttack, SpAttack);
        public int BoostedSpDefence => GetBoostedStat(Stat.SpDefence, SpDefence);
        public int BoostedSpeed => GetBoostedStat(Stat.Speed, Speed);

        public Dictionary<Stat, int> StatBoosts { get; private set; }
        private static float[] _statChangePenalties = {1.0f, 0.66f, 0.5f, 0.4f, 0.33f, 0.285f, 0.25f}; 
        private static float[] _statChangeBonuses = {1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f};

        private int GetBoostedStat(Stat statType, int statValue) => (int) (StatBoosts[statType] < 0
            ? statValue * _statChangePenalties[Mathf.Abs(StatBoosts[statType])]
            : statValue * _statChangeBonuses[Mathf.Abs(StatBoosts[statType])]);

        public void ApplyStatChange(Stat stat, int steps) =>
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + steps, -6, 6);

        public bool ApplyPrimaryCondition(PrimaryStatusCondition newCondition)
        {
            if (PrimaryCondition != PrimaryStatusCondition.None) return false;
            PrimaryCondition = newCondition;
            return true;
        }
        
        public void ApplySecondaryCondition(SecondaryStatusCondition newCondition)
        {
            if (SecondaryConditions.Contains(newCondition)) return;
            SecondaryConditions.Add(newCondition);
        }
    }
}