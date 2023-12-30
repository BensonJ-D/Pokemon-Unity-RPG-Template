using System;
using Characters.Moves;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class LearnableMove
    {
        [SerializeField] private MoveBase moveBase;
        [SerializeField] private int level;

        public LearnableMove(MoveBase moveBase, int level) {
            this.moveBase = moveBase;
            this.level = level;
        }

        public MoveBase Base => moveBase;
        public int Level => level;
    }
}