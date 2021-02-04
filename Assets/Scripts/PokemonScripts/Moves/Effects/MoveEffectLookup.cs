using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PokemonScripts.Moves.Effects
{

    public static class PrimaryStatuses {
        public enum EffectType { None, PoisonTarget, BurnTarget, ParalyseTarget, FreezeTarget, SleepTarget }

        public static readonly ReadOnlyDictionary<EffectType, MoveEffect> GetEffectClass =
            new ReadOnlyDictionary<EffectType, MoveEffect>(
                new Dictionary<EffectType, MoveEffect>()
                {
                    { EffectType.PoisonTarget, new PoisonTarget() },
                    { EffectType.BurnTarget, new BurnTarget() },
                    { EffectType.ParalyseTarget, new ParalyseTarget() },
                    { EffectType.FreezeTarget, new FreezeTarget() },
                    { EffectType.SleepTarget, new SleepTarget() }
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