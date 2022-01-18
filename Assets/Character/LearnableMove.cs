using System;
using PokemonScripts.Moves;
using UnityEngine;

namespace Character
{
    [Serializable]
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