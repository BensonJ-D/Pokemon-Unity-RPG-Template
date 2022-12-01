// using System;
// using Battle;
// using PokemonScripts.Conditions;
// using UnityEngine;
//
// namespace PokemonScripts.Moves.Effects
// {
//     [Serializable]
//     public class SecondaryStatusEffect
//     {
//         [SerializeField] private SecondaryStatusEffects.EffectType statusCondition;
//
//         public SecondaryStatusEffects.EffectType StatusCondition => statusCondition;
//     }
//     
//     public class ConfuseTarget : ModifySecondaryStatus
//     {
//         public override string ApplyEffect(BattlePokemon user, BattlePokemon target) { 
//             var success = ApplySecondaryCondition(user, target,  SecondaryStatusCondition.Confusion);
//             return $"{target.Pokemon.Name} became confused!";
//         }
//     }
//     
//     public class FlinchTarget : ModifySecondaryStatus
//     {
//         public override string ApplyEffect(BattlePokemon user, BattlePokemon target) { 
//             var success = ApplySecondaryCondition(user, target,  SecondaryStatusCondition.Flinched);
//             return $"{target.Pokemon.Name} flinched and was unable to act!";
//         }
//     }
// }