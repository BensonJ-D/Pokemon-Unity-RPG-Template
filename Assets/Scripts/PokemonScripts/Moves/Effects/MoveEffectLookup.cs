using System.Collections.Generic;
using System.Collections.ObjectModel;
using PokemonScripts.Conditions;

namespace PokemonScripts.Moves.Effects
{

    public static class PrimaryStatuses {
        public enum EffectType { 
            None = PrimaryConditions.PrimaryStatusCondition.None, PoisonTarget = PrimaryConditions.PrimaryStatusCondition.Poison, 
            BurnTarget = PrimaryConditions.PrimaryStatusCondition.Burn, ParalyseTarget = PrimaryConditions.PrimaryStatusCondition.Paralyse, 
            FreezeTarget = PrimaryConditions.PrimaryStatusCondition.Freeze, SleepTarget = PrimaryConditions.PrimaryStatusCondition.Sleep }

        public static readonly ReadOnlyDictionary<EffectType, MoveEffect> GetEffectClass =
            new ReadOnlyDictionary<EffectType, MoveEffect>(
                new Dictionary<EffectType, MoveEffect>()
                {
                    {EffectType.None, null },
                    { EffectType.PoisonTarget, new PoisonTarget() },
                    { EffectType.BurnTarget, new BurnTarget() },
                    { EffectType.ParalyseTarget, new ParalyseTarget() },
                    { EffectType.FreezeTarget, new FreezeTarget() },
                    { EffectType.SleepTarget, new SleepTarget() }
                }
            );
    }
    
    
    public static class SecondaryStatuses {
        public enum EffectType { None, ConfuseTarget = SecondaryConditions.SecondaryStatusCondition.Confusion, 
            FlinchTarget = SecondaryConditions.SecondaryStatusCondition.Flinched }

        public static readonly ReadOnlyDictionary<EffectType, MoveEffect> GetEffectClass =
            new ReadOnlyDictionary<EffectType, MoveEffect>(
                new Dictionary<EffectType, MoveEffect>()
                {
                    { EffectType.ConfuseTarget, new ConfuseTarget() },
                    { EffectType.FlinchTarget, new FlinchTarget() }
                }
            );
    }
    
    public static class StatModifiers {
        public enum EffectType { ModifyAttack = Stat.Attack, 
            ModifyDefence = Stat.Defence, ModifySpAttack = Stat.SpAttack, 
            ModifySpDefence = Stat.SpDefence, ModifySpeed = Stat.Speed }
        
        public static readonly ReadOnlyDictionary<EffectType, MoveEffect> GetEffectClass =
            new ReadOnlyDictionary<EffectType, MoveEffect>(
                new Dictionary<EffectType, MoveEffect>()
                {
                    { EffectType.ModifyAttack, new ModifyAttack() },
                    { EffectType.ModifyDefence, new ModifyDefence() },
                    { EffectType.ModifySpAttack, new ModifySpAttack() },
                    { EffectType.ModifySpDefence, new ModifySpDefence() },
                    { EffectType.ModifySpeed, new ModifySpeed() },
                    // { EffectType.ModifyAccuracy, new ModifyAccuracy() },
                    // { EffectType.ModifyEvasion, new ModifyEvasion() },
                }
            );
    }
}