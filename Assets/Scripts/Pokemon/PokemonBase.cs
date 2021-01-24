using System.Collections.Generic;
using UnityEngine;

namespace Pokemon
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

        [SerializeField] private LearnableMove[] learnableMoves;

        public void InitialiseInstance(Sprite _front, Sprite _back, int _number, string _name, PokemonType _type1, PokemonType _type2, 
            int _maxHP, int _attack, int _defence, int _spAttack, int _spDefence, int _speed, List<LearnableMove> _learnableMoves)
        {
            this.front = _front;
            this.back = _back;
            this.number = _number;
            this.species = _name;
            this.type1 = _type1;
            this.type2 = _type2;
            this.maxHp = _maxHP;
            this.attack = _attack;
            this.defence = _defence;
            this.spAttack = _spAttack;
            this.spDefence = _spDefence;
            this.speed = _speed;
        
            learnableMoves = new LearnableMove[_learnableMoves.Count];
            for (var i = 0; i < _learnableMoves.Count; i++)
            {
                learnableMoves[i] = _learnableMoves[i];
            }
        }

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