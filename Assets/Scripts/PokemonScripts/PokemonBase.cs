using System.Collections.Generic;
using UnityEngine;

namespace PokemonScripts
{
    [CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
    public class PokemonBase : ScriptableObject
    {
        [SerializeField] private string species;

        [TextArea] [SerializeField] private string description;

        [SerializeField] private Sprite front;
        [SerializeField] private Sprite back;

        [SerializeField] private int number;
        [SerializeField] private PokemonType type1;
        [SerializeField] private PokemonType type2;

        [SerializeField] private int maxHp;
        [SerializeField] private int attack;
        [SerializeField] private int defence;
        [SerializeField] private int spAttack;
        [SerializeField] private int spDefence;
        [SerializeField] private int speed;
        
        [SerializeField] private ExperienceGroup experienceGroup;
        [SerializeField] private int experienceYield;

        [SerializeField] private LearnableMove[] learnableMoves;

        public int Number => number;
        public string Species => species;
        public string Description => description;
        public Sprite FrontSprite => front;
        public Sprite BackSprite => back;
        public PokemonType Type1 => type1;
        public PokemonType Type2 => type2;
        public int MaxHp => maxHp;
        public int Attack => attack;
        public int Defence => defence;
        public int SpAttack => spAttack;
        public int SpDefence => spDefence;
        public int Speed => speed;

        public ExperienceGroup ExperienceGroup => experienceGroup;
        public int ExperienceYield => experienceYield;
        public LearnableMove[] LearnableMoves => learnableMoves;
    }


    [System.Serializable]
    public class LearnableMove
    {
        [SerializeField] private MoveBase moveBase;
        [SerializeField] private int level;

        public LearnableMove(MoveBase moveBase, int level)
        {
            this.moveBase = moveBase;
            this.level = level;
        }
        public MoveBase Base => moveBase;
        public int Level => level;
    }
}