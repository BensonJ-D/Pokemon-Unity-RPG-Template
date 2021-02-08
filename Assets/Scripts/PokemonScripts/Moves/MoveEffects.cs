using UnityEngine;

namespace PokemonScripts.Moves
{
    public class MoveEffect
    {
        public virtual string ApplyEffect(Pokemon user, Pokemon target) { return ""; }
        public virtual string ApplyEffect(Pokemon user, Pokemon target, object effect1) { return ""; }
        public virtual string ApplyEffect(Pokemon user, Pokemon target, object effect1, object effect2) { return ""; }
    }
    
    public class ModifyStatus : MoveEffect
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, object effect1, object effect)
        {
            return "";
        }
    }
    
    public class ModifyStat : MoveEffect
    {
        private readonly string[] increasedPhrase = { "", "rose", "rose sharply", "rose drastically" };
        private readonly string[] decreasedPhrase = { "", "fell", "fell harshly", "fell severely" };
        private readonly string statMinOrMaxPhrase = "won't go any ";
            
        public override string ApplyEffect(Pokemon user, Pokemon target, object effect1, object effect2)
        {
            var stat = (Stat) effect1;
            var modifier = (int) effect2;
            var message = $"{target.Base.Species}'s {stat} ";
            var currentLevel = target.StatBoosts[stat];
            var decreasing = modifier < 0;
            if (Mathf.Abs(currentLevel) == 6)
            {
                message += statMinOrMaxPhrase;
                message += decreasing ? "lower!" : "higher!";
            }
            else
            {
                var phraseIndex = Mathf.Abs(modifier);
                message += decreasing ? decreasedPhrase[phraseIndex] : increasedPhrase[phraseIndex];
            }

            target.StatBoosts[(Stat) effect1] = 
                Mathf.Clamp(target.StatBoosts[stat] + modifier, -6, 6);

            return message;
        }
    }
}