// using System;
// using Battle;
// using Characters.Monsters;
// using Characters.Moves;
// using UnityEngine;
//
// namespace Menus.Move.Effects
// {
//     [Serializable]
//     public class StatModifierEffect
//     {
//         [SerializeField] private MoveTarget target;
//         [SerializeField] private StatModifierEffects.EffectType stat;
//         [SerializeField] private int modifier;
//
//         public MoveTarget Target => target;
//         // public StatModifierEffects.EffectType Stat => stat;
//         public int Modifier => modifier;
//     }
//     
//     public class ModifyAttack : ModifyStat
//     {
//         public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
//             return base.ApplyEffect(user, target,  Stat.Attack, modifier);
//         }
//     }
//     
//     public class ModifyDefence : ModifyStat
//     {
//         public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
//             return base.ApplyEffect(user, target,  Stat.Defence, modifier);
//         }
//     }
//
//     public class ModifySpAttack : ModifyStat
//     {
//         public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
//             return base.ApplyEffect(user, target, Stat.SpAttack, modifier);
//         }
//     }
//
//     public class ModifySpDefence : ModifyStat
//     {
//         public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
//             return base.ApplyEffect(user, target,  Stat.SpDefence, modifier);
//         }
//     }
//     
//     public class ModifySpeed : ModifyStat
//     {
//         public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
//             return base.ApplyEffect(user, target,  Stat.Speed, modifier);
//         }
//     }
// }

