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
            var message = $"{target.Base.Species}'s {(Stat) effect1} ";
            var currentLevel = target.StatBoosts[(Stat) effect1];
            var decreasing = currentLevel < 0;
            if (Mathf.Abs(currentLevel) == 6)
            {
                message += statMinOrMaxPhrase;
                message += decreasing ? "lower!" : "higher!";
            }
            else
            {
                var phraseIndex = Mathf.Clamp(Mathf.Abs(currentLevel), 0, 3);
                message += decreasing ? decreasedPhrase[phraseIndex] : increasedPhrase[phraseIndex];
            }

            target.StatBoosts[(Stat) effect1] = 
                Mathf.Clamp(target.StatBoosts[(Stat) effect1] + (int) effect2, -6, 6);

            return message;
        }
    }
}