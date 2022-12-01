using Characters.Monsters;
using PokemonScripts.Conditions;
using UnityEngine;

namespace Characters.Moves
{
    public class MoveEffect
    {
        public virtual string ApplyEffect(Pokemon user, Pokemon target) { return ""; }
        public virtual string ApplyEffect(Pokemon user, Pokemon target, object effect1) { return ""; }
        public virtual string ApplyEffect(Pokemon user, Pokemon target, object effect1, object effect2) { return ""; }
    }
    
    // public class ModifyPrimaryStatus : MoveEffect
    // {
    //     protected static bool ApplyPrimaryCondition(Pokemon user, Pokemon target, PrimaryStatusCondition condition)
    //     {
    //         if (target.CurrentHp <= 0) return false;
    //         
    //         var success = target.ApplyPrimaryCondition(condition);
    //         return success;
    //     }
    // }
    
    // public class ModifySecondaryStatus : MoveEffect
    // {
    //     protected static bool ApplySecondaryCondition(Pokemon user, Pokemon target, SecondaryStatusCondition condition)
    //     {
    //         target.ApplySecondaryCondition(condition);
    //         return false;
    //     }
    // }
    //
    // public class ModifyStat : MoveEffect
    // {
    //     private readonly string[] _increasedPhrase = { "", "rose", "rose sharply", "rose drastically" };
    //     private readonly string[] _decreasedPhrase = { "", "fell", "fell harshly", "fell severely" };
    //     private const string StatMinOrMaxPhrase = "won't go any ";
    //
    //     public override string ApplyEffect(Pokemon user, Pokemon target, object effect1, object effect2)
    //     {
    //         if (target.CurrentHp <= 0) return "";
    //         
    //         var stat = (Stat) effect1;
    //         var modifier = (int) effect2;
    //         var message = $"{target.Base.Species}'s {stat} ";
    //         var currentLevel = target.StatBoosts[stat];
    //         var decreasing = modifier < 0;
    //         if (Mathf.Abs(currentLevel) == 6)
    //         {
    //             message += StatMinOrMaxPhrase;
    //             message += decreasing ? "lower!" : "higher!";
    //         }
    //         else
    //         {
    //             var phraseIndex = Mathf.Abs(modifier);
    //             message += decreasing ? _decreasedPhrase[phraseIndex] : _increasedPhrase[phraseIndex];
    //         }
    //
    //         target.StatBoosts[(Stat) effect1] = 
    //             Mathf.Clamp(target.StatBoosts[stat] + modifier, -6, 6);
    //
    //         return message;
    //     }
    // }
}