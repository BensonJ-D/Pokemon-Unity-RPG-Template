using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PokemonScripts.Moves.Effects
{

    public static class PrimaryStatusEffects {
        public enum EffectType { 
            None = PrimaryStatusCondition.None, PoisonTarget = PrimaryStatusCondition.Poison, 
            BurnTarget = PrimaryStatusCondition.Burn, ParalyseTarget = PrimaryStatusCondition.Paralyse, 
            FreezeTarget = PrimaryStatusCondition.Freeze, SleepTarget = PrimaryStatusCondition.Sleep }

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
    
    
    public static class SecondaryStatusEffects {
        public enum EffectType { 
            None = SecondaryStatusCondition.None, ConfuseTarget = SecondaryStatusCondition.Confusion, 
            FlinchTarget = SecondaryStatusCondition.Flinched }

        public static readonly ReadOnlyDictionary<EffectType, MoveEffect> GetEffectClass =
            new ReadOnlyDictionary<EffectType, MoveEffect>(
                new Dictionary<EffectType, MoveEffect>()
                {
                    { EffectType.ConfuseTarget, new ConfuseTarget() },
                    { EffectType.FlinchTarget, new FlinchTarget() }
                }
            );
    }
    
    public static class StatModifierEffects {
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