// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.ObjectModel;
// using Battle;
// using Misc;
// using UnityEngine;
//
// namespace PokemonScripts.Conditions
// {
//     public enum PrimaryStatusCondition { None, Poison, Burn, Paralyse, Freeze, Sleep }
//
//     public static class PrimaryStatusConditions
//     {
//         public static readonly ReadOnlyDictionary<PrimaryStatusCondition, StatusCondition> GetEffectClass =
//             new ReadOnlyDictionary<PrimaryStatusCondition, StatusCondition>(
//                 new Dictionary<PrimaryStatusCondition, StatusCondition>
//                 {
//                     { PrimaryStatusCondition.None, new NoStatusCondition() },
//                     { PrimaryStatusCondition.Poison, new PoisonCondition() },
//                     { PrimaryStatusCondition.Burn, new BurnCondition() }
//                 }
//             );
//     }
//
//     public class NoStatusCondition : StatusCondition { }
//
//     public class PoisonCondition : StatusCondition
//     {
//         public override IEnumerator OnAfterTurn(BattlePokemon battlePokemon, BattleDialogBox battleDialogBox)
//         {
//             var pokemon = battlePokemon.Pokemon;
//             var damage = pokemon.MaxHp() / 8;
//             var fainted = damage >= pokemon.CurrentHp;
//             var dmgDetails = new DamageDetails(fainted, damage);
//             var updateHealthBar = new Task(battlePokemon.UpdateHealth(dmgDetails));
//             var playDamageAnimation = new Task(battlePokemon.PlayDamageAnimation());
//             yield return new WaitWhile(() => updateHealthBar.Running || playDamageAnimation.Running);
//             yield return battleDialogBox.TypeMessage($"{pokemon.Name} is hurt by poison!");
//         }
//     }
//     
//     public class BurnCondition : StatusCondition
//     {
//         public override IEnumerator OnAfterTurn(BattlePokemon battlePokemon, BattleDialogBox battleDialogBox)
//         {
//             var pokemon = battlePokemon.Pokemon;
//             var damage = pokemon.MaxHp() / 8;
//             var fainted = damage >= pokemon.CurrentHp;
//             var dmgDetails = new DamageDetails(fainted, damage);
//             var updateHealthBar = new Task(battlePokemon.UpdateHealth(dmgDetails));
//             var playDamageAnimation = new Task(battlePokemon.PlayDamageAnimation());
//             yield return new WaitWhile(() => updateHealthBar.Running || playDamageAnimation.Running);
//             yield return battleDialogBox.TypeMessage($"{pokemon.Name} is hurt by burn!");
//         }
//     }
// }

