using System;
using UnityEngine;

namespace PokemonScripts.Moves.Effects
{
    [Serializable]
    public class PrimaryStatusEffect
    {
        [SerializeField] private PrimaryStatuses.EffectType statusCondition;

        public PrimaryStatuses.EffectType StatusCondition => statusCondition;
    }
    
    public class PoisonTarget : ModifyStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryStatusCondition.Poison);
            return $"{target.Base.Species} was poisoned!";
        }
    }
    
    public class BurnTarget : ModifyStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryStatusCondition.Burn);
            return $"{target.Base.Species} was burned!";
        }
    }

    public class ParalyseTarget : ModifyStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryStatusCondition.Paralyse);
            return $"{target.Base.Species} was paralysed!";
        }
    }

    public class FreezeTarget : ModifyStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryStatusCondition.Freeze);
            return $"{target.Base.Species} was frozen!";
        }
    }
    
    public class SleepTarget : ModifyStatus
    {
        public override string ApplyEffect(Pokemon user, Pokemon target) { 
            base.ApplyEffect(user, target, (int) PrimaryStatusCondition.Sleep);
            return $"{target.Base.Species} fell asleep!";
        }
    }
}