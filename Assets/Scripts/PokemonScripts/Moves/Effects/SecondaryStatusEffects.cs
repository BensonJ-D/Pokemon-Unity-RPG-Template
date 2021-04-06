using System;
using UnityEngine;

namespace PokemonScripts.Moves.Effects
{
    [Serializable]
    public class SecondaryStatusEffect
    {
        [SerializeField] private SecondaryStatusEffects.EffectType statusCondition;

        public SecondaryStatusEffects.EffectType StatusCondition => statusCondition;
    }
    
    public class ConfuseTarget : ModifySecondaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplySecondaryCondition(user, target,  SecondaryStatusCondition.Confusion);
            return $"{target.Name} became confused!";
        }
    }
    
    public class FlinchTarget : ModifySecondaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplySecondaryCondition(user, target,  SecondaryStatusCondition.Flinched);
            return $"{target.Name} flinched and was unable to act!";
        }
    }
}