using Battle;
using PokemonScripts.Conditions;
using UnityEngine;

namespace PokemonScripts.Moves
{
    public class MoveEffect
    {
        public virtual string ApplyEffect(BattlePokemon user, BattlePokemon target) { return ""; }
        public virtual string ApplyEffect(BattlePokemon user, BattlePokemon target, object effect1) { return ""; }
        public virtual string ApplyEffect(BattlePokemon user, BattlePokemon target, object effect1, object effect2) { return ""; }
    }
    
    public class ModifyPrimaryStatus : MoveEffect
    {
        protected static bool ApplyPrimaryCondition(BattlePokemon user, BattlePokemon target, PrimaryStatusCondition condition)
        {
            if (target.Pokemon.CurrentHp <= 0) return false;
            
            var success = target.Pokemon.ApplyPrimaryCondition(condition);
            target.UpdateStatus();
            return success;
        }
    }
    
    public class ModifySecondaryStatus : MoveEffect
    {
        protected static bool ApplySecondaryCondition(BattlePokemon user, BattlePokemon target, SecondaryStatusCondition condition)
        {
            target.Pokemon.ApplySecondaryCondition(condition);
            return false;
        }
    }
    
    public class ModifyStat : MoveEffect
    {
        private readonly string[] _increasedPhrase = { "", "rose", "rose sharply", "rose drastically" };
        private readonly string[] _decreasedPhrase = { "", "fell", "fell harshly", "fell severely" };
        private const string StatMinOrMaxPhrase = "won't go any ";

        public override string ApplyEffect(BattlePokemon user, BattlePokemon target, object effect1, object effect2)
        {
            if (target.Pokemon.CurrentHp <= 0) return "";
            
            var stat = (Stat) effect1;
            var modifier = (int) effect2;
            var targetPokemon = target.Pokemon;
            var message = $"{targetPokemon.Base.Species}'s {stat} ";
            var currentLevel = targetPokemon.StatBoosts[stat];
            var decreasing = modifier < 0;
            if (Mathf.Abs(currentLevel) == 6)
            {
                message += StatMinOrMaxPhrase;
                message += decreasing ? "lower!" : "higher!";
            }
            else
            {
                var phraseIndex = Mathf.Abs(modifier);
                message += decreasing ? _decreasedPhrase[phraseIndex] : _increasedPhrase[phraseIndex];
            }

            targetPokemon.StatBoosts[(Stat) effect1] = 
                Mathf.Clamp(targetPokemon.StatBoosts[stat] + modifier, -6, 6);

            return message;
        }
    }
}