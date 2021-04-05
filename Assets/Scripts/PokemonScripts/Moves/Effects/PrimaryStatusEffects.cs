using System;
using PokemonScripts.Conditions;
using UnityEngine;

namespace PokemonScripts.Moves.Effects
{
    [Serializable]
    public class PrimaryStatusEffect
    {
        [SerializeField] private PrimaryStatuses.EffectType statusCondition;

        public PrimaryStatuses.EffectType StatusCondition => statusCondition;
    }
    
    public class PoisonTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplyPrimaryCondition(user, target,  PrimaryConditions.PrimaryStatusCondition.Poison);
            return success ? $"{target.Base.Species} was poisoned!" : "It had no effect ...";
        }
    }
    
    public class BurnTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplyPrimaryCondition(user, target,  PrimaryConditions.PrimaryStatusCondition.Burn);
            return success ? $"{target.Base.Species} was burned!" : "It had no effect ...";
        }
    }

    public class ParalyseTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplyPrimaryCondition(user, target,  PrimaryConditions.PrimaryStatusCondition.Paralyse);
            return success ? $"{target.Base.Species} was paralysed!" : "It had no effect ...";
        }
    }

    public class FreezeTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplyPrimaryCondition(user, target,  PrimaryConditions.PrimaryStatusCondition.Freeze);
            return success ? $"{target.Base.Species} was frozen!" : "It had no effect ...";
        }
    }
    
    public class SleepTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            var success = ApplyPrimaryCondition(user, target, PrimaryConditions.PrimaryStatusCondition.Sleep);
            return success ? $"{target.Base.Species} fell asleep!" : "It had no effect ...";
        }
    }
}