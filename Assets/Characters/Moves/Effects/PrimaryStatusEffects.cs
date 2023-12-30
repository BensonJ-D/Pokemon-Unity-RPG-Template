using System;

namespace Menus.Move.Effects
{
    [Serializable]
    public class PrimaryStatusEffect
    {
        // [SerializeField] private PrimaryStatusEffects.EffectType statusCondition;

        // public PrimaryStatusEffects.EffectType StatusCondition => statusCondition;
    }

    // public class PoisonTarget : ModifyPrimaryStatus
    // {
    //     public override string ApplyEffect(Pokemon user, Pokemon target) { 
    //         var success = ApplyPrimaryCondition(user, target,  PrimaryStatusCondition.Poison);
    //         return success ? $"{target.Base.Species} was poisoned!" : "It had no effect ...";
    //     }
    // }
    //
    // public class BurnTarget : ModifyPrimaryStatus
    // {
    //     public override string ApplyEffect(Pokemon user, Pokemon target) { 
    //         var success = ApplyPrimaryCondition(user, target,  PrimaryStatusCondition.Burn);
    //         return success ? $"{target.Base.Species} was burned!" : "It had no effect ...";
    //     }
    // }
    //
    // public class ParalyseTarget : ModifyPrimaryStatus
    // {
    //     public override string ApplyEffect(Pokemon user, Pokemon target) { 
    //         var success = ApplyPrimaryCondition(user, target,  PrimaryStatusCondition.Paralyse);
    //         return success ? $"{target.Base.Species} was paralysed!" : "It had no effect ...";
    //     }
    // }
    //
    // public class FreezeTarget : ModifyPrimaryStatus
    // {
    //     public override string ApplyEffect(Pokemon user, Pokemon target) { 
    //         var success = ApplyPrimaryCondition(user, target,  PrimaryStatusCondition.Freeze);
    //         return success ? $"{target.Base.Species} was frozen!" : "It had no effect ...";
    //     }
    // }
    //
    // public class SleepTarget : ModifyPrimaryStatus
    // {
    //     public override string ApplyEffect(Pokemon user, Pokemon target) { 
    //         var success = ApplyPrimaryCondition(user, target, PrimaryStatusCondition.Sleep);
    //         return success ? $"{target.Base.Species} fell asleep!" : "It had no effect ...";
    //     }
    // }
}