using System;
using PokemonScripts.Conditions;
using UnityEngine;

namespace PokemonScripts.Moves.Effects
{
    [Serializable]
    public class SecondaryStatusEffect
    {
        [SerializeField] private SecondaryStatuses.EffectType statusCondition;

        public SecondaryStatuses.EffectType StatusCondition => statusCondition;
    }
    
    public class ConfuseTarget : ModifySecondaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplySecondaryCondition(user, target,  SecondaryConditions.SecondaryStatusCondition.Confusion);
            return $"{target.Name} became confused!";
        }
    }
    
    public class FlinchTarget : ModifySecondaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplySecondaryCondition(user, target,  SecondaryConditions.SecondaryStatusCondition.Flinched);
            return $"{target.Name} flinched and was unable to act!";
        }
    }
}