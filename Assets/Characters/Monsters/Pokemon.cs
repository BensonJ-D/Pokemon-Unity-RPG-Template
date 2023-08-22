using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Characters.Moves;
using Characters.UI;
using PokemonScripts.Conditions;
using Popup;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Characters.Monsters
{
    public enum Stat { Hp, Attack, Defence, SpAttack, SpDefence, Speed, Accuracy, Evasion }

    [Serializable]
    [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
    public class Pokemon
    {
        [SerializeField] private PokemonBase pokemonBase;
        [SerializeField] private float baseHealth;
        [SerializeField] private float baseExp;
        [SerializeField] private int initialLevel;
        
        private const int MinimumLevel = 1;
        private const int MaximumLevel = 100;

        private CharacterStatus _statusUI;

        // [SerializeField] private ExperienceBar experienceBar;

        public void Initialization()
        {
            Initialization(pokemonBase, initialLevel, baseHealth);
        }

        public void Initialization(PokemonBase @base, int level, float health = 100f)
        {
            Base = @base;
            Level = level;
            Name = @base.Species;

            CurrentHp = (int) (MaxHp() * (health / 100));

            var percentageToNextLevel = (int) ((NextLevelExperience - BaseLevelExperience) * (baseExp / 100));
            CurrentExperience = BaseLevelExperience + percentageToNextLevel;

            Moves = new List<Move>();
            foreach (var move in Base.LearnableMoves)
            {
                if (move.Level > Level) break;

                if (Moves.Exists(oldMove => move.Base.Number == oldMove.Base.Number)) continue;

                var newMove = new Move(move.Base);
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

        public PokemonBase Base { get; private set; }
        public bool IsFainted => CurrentHp <= 0;

        public int Level { get; private set; }
        public int CurrentExperience { get; set; }

        public int BaseLevelExperience => Level switch
            {
                MaximumLevel => ExperienceGroups.GetExperienceList[Base.ExperienceGroup][Level - 2],
                _            => ExperienceGroups.GetExperienceList[Base.ExperienceGroup][Level - 1]
            };

        public int NextLevelExperience => Level switch
            {
                MaximumLevel => ExperienceGroups.GetExperienceList[Base.ExperienceGroup][Level - 1],
                _            => ExperienceGroups.GetExperienceList[Base.ExperienceGroup][Level]
            };

        // public PrimaryStatusCondition PrimaryCondition { get; private set; } = PrimaryStatusCondition.None;
        // public List<SecondaryStatusCondition> SecondaryConditions { get; private set; } = new List<SecondaryStatusCondition>();
        
        private int MaxHp(int level) => Mathf.FloorToInt((2 * Base.MaxHp * level) / 100f) + 10 + level;
        private int Attack(int level) => Mathf.FloorToInt((2 * Base.Attack * level) / 100f) + 5; 
        private int Defence(int level) => Mathf.FloorToInt((2 * Base.Defence * level) / 100f) + 5; 
        private int SpAttack(int level) => Mathf.FloorToInt((2 * Base.SpAttack * level) / 100f) + 5;
        private int SpDefence(int level) => Mathf.FloorToInt((2 * Base.SpDefence * level) / 100f) + 5; 
        private int Speed(int level) => Mathf.FloorToInt((2 * Base.Speed * level) / 100f) + 5; 

        public int MaxHp() => MaxHp(Level);
        public int Attack() => Attack(Level);
        public int Defence() => Defence(Level);
        public int SpAttack() => SpAttack(Level);
        public int SpDefence() => SpDefence(Level);
        public int Speed() => Speed(Level);

        public List<Move> Moves { get; private set; }

        public int CurrentHp { get; set; }
        public int BoostedAttack => GetBoostedStat(Stat.Attack, Attack());
        public int BoostedDefence => GetBoostedStat(Stat.Defence, Defence());
        public int BoostedSpAttack => GetBoostedStat(Stat.SpAttack, SpAttack());
        public int BoostedSpDefence => GetBoostedStat(Stat.SpDefence, SpDefence());
        public int BoostedSpeed => GetBoostedStat(Stat.Speed, Speed());

        public int ExperienceYield => Mathf.FloorToInt(Level * Base.ExperienceYield / 7f);

        public Dictionary<Stat, int> StatBoosts { get; private set; }
        private static float[] _statChangePenalties = {1.0f, 0.66f, 0.5f, 0.4f, 0.33f, 0.285f, 0.25f}; 
        private static float[] _statChangeBonuses = {1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f};

        private int GetBoostedStat(Stat statType, int statValue) => (int) (StatBoosts[statType] < 0
            ? statValue * _statChangePenalties[Mathf.Abs(StatBoosts[statType])]
            : statValue * _statChangeBonuses[Mathf.Abs(StatBoosts[statType])]);

        public void ApplyStatChange(Stat stat, int steps) =>
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + steps, -6, 6);

        // public bool ApplyPrimaryCondition(PrimaryStatusCondition newCondition)
        // {
        //     if (PrimaryCondition != PrimaryStatusCondition.None) return false;
        //     PrimaryCondition = newCondition;
        //     return true;
        // }
        //
        // public void ApplySecondaryCondition(SecondaryStatusCondition newCondition)
        // {
        //     if (SecondaryConditions.Contains(newCondition)) return;
        //     SecondaryConditions.Add(newCondition);
        // }

        public bool CheckForLevel()
        {
            if (CurrentExperience < NextLevelExperience) return false;
            
            ++Level;
            return true;
        }

        public Stats GetStats()
        {
            return new Stats
            {
                MaxHp = MaxHp(),
                Attack = Attack(),
                Defence = Defence(),
                SpAttack = SpAttack(),
                SpDefence = SpDefence(),
                Speed = Speed()
            };
        }
        
        public Stats GetStats(int level)
        {
            return new Stats
            {
                MaxHp = MaxHp(level),
                Attack = Attack(level),
                Defence = Defence(level),
                SpAttack = SpAttack(level),
                SpDefence = SpDefence(level),
                Speed = Speed(level)
            };
        }

        public CharacterStatus StatusUI
        {
            get => _statusUI;
            set
            {
                _statusUI = value;

                if (_statusUI?.Name) _statusUI.Name.text = Name;

                _statusUI?.HealthBar?.SetValue(0, CurrentHp, MaxHp());
                _statusUI?.ExpBar?.SetValue(BaseLevelExperience, CurrentExperience, NextLevelExperience);
                
                if (_statusUI?.Level) _statusUI.Level.text = Level.ToString();
                UpdateStats();
            }
        }

        private void UpdateStats()
        {
            if (_statusUI?.Stats == null) return;
            
            _statusUI.Stats.AttackText.text = Attack().ToString();
            _statusUI.Stats.DefenceText.text = Defence().ToString();
            _statusUI.Stats.SpAtkText.text = SpAttack().ToString();
            _statusUI.Stats.SpDefText.text = SpDefence().ToString();
            _statusUI.Stats.SpeedText.text = Speed().ToString();
        }

        private void SetLevel(int level)
        {
            var updatedHealth = level != Level ? CurrentHp + MaxHp(level) - MaxHp(Level) : CurrentHp;

            Level = level;
            UpdateStats();
            SetHealth(updatedHealth);
            if (_statusUI?.Level) _statusUI.Level.text = Level.ToString();
        }
        
        public void SetHealth(int newHealth)
        {
            CurrentHp = Mathf.Clamp(newHealth, 0, MaxHp());

            _statusUI?.HealthBar?.SetValue(newHealth);
        }
        
        public IEnumerator UpdateHealth(int healthAdjustment, uint speedMultiplier = 100)
        {
            CurrentHp = Mathf.Clamp(healthAdjustment + CurrentHp, 0, MaxHp());
            
            yield return _statusUI?.HealthBar?.UpdateBar(healthAdjustment, speedMultiplier);
        }

        public void SetExp(int newExp)
        {
            CurrentExperience = Mathf.Clamp(newExp, 0, ExperienceGroups.GetExperienceList[Base.ExperienceGroup][100]);
            while (CurrentExperience < BaseLevelExperience) SetLevel(Level - 1);
            while (CurrentExperience > NextLevelExperience) SetLevel(Level + 1);

            SetHealth(CurrentHp);
            _statusUI?.HealthBar?.SetValue(newExp);
        }
        
        public IEnumerator UpdateExp(int expAdjustment, uint speedMultiplier = 500)
        {
            var isPositiveDelta = expAdjustment > 0;
            var targetExp = CurrentExperience + expAdjustment;
            do
            {
                var delta = targetExp - CurrentExperience;
                CurrentExperience = Mathf.Clamp(targetExp, BaseLevelExperience, NextLevelExperience);
                
                yield return _statusUI?.ExpBar?.UpdateBar(delta, speedMultiplier);
                
                if (CurrentExperience > BaseLevelExperience && CurrentExperience < NextLevelExperience) continue;
                
                if (CurrentExperience <= BaseLevelExperience && Level > MinimumLevel) SetLevel(Level - 1);
                else if (CurrentExperience >= NextLevelExperience && Level < MaximumLevel) SetLevel(Level + 1);

                if (Level == 100 && isPositiveDelta) targetExp = NextLevelExperience;
                if (Level == 1 && !isPositiveDelta) targetExp = BaseLevelExperience;

                _statusUI?.HealthBar?.SetValue(0, CurrentHp, MaxHp());
                _statusUI?.ExpBar?.SetValue(BaseLevelExperience, CurrentExperience, NextLevelExperience);
            } 
            while (isPositiveDelta ? CurrentExperience < targetExp : CurrentExperience > targetExp);
        }
    }
}