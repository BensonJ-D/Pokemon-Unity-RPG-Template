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
            base.ApplyEffect(user, target,  PrimaryConditions.PrimaryStatusCondition.Poison);
            return $"{target.Base.Species} was poisoned!";
        }
    }
    
    public class BurnTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target,  PrimaryConditions.PrimaryStatusCondition.Burn);
            return $"{target.Base.Species} was burned!";
        }
    }

    public class ParalyseTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target,  PrimaryConditions.PrimaryStatusCondition.Paralyse);
            return $"{target.Base.Species} was paralysed!";
        }
    }

    public class FreezeTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target,  PrimaryConditions.PrimaryStatusCondition.Freeze);
            return $"{target.Base.Species} was frozen!";
        }
    }
    
    public class SleepTarget : ModifyPrimaryStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryConditions.PrimaryStatusCondition.Sleep);
            return $"{target.Base.Species} fell asleep!";
        }
    }
}