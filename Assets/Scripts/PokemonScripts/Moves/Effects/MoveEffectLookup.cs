using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PokemonScripts.Moves.Effects
{
    public enum EffectType {
        PoisonTarget, BurnTarget, ParalyseTarget, FreezeTarget,
        LowerTargetsAttack, RaiseTargetsAttack, LowerUsersAttack, RaiseUsersAttack,
        LowerTargetsDefence, RaiseTargetsDefence, LowerUsersDefence, RaiseUsersDefence,
        LowerTargetsSpAttack, RaiseTargetsSpAttack, LowerUsersSpAttack, RaiseUsersSpAttack,
        LowerTargetsSpDefence, RaiseTargetsSpDefence, LowerUsersSpDefence, RaiseUsersSpDefence,
        LowerTargetsSpeed, RaiseTargetsSpeed, LowerUsersSpeed, RaiseUsersSpeed
    }
    public static class MoveEffectLookup {
        public static readonly ReadOnlyDictionary<EffectType, IMoveEffect> MoveEffectFunctions =
            new ReadOnlyDictionary<EffectType, IMoveEffect>(
                new Dictionary<EffectType, IMoveEffect>()
                {
                    { EffectType.PoisonTarget, new PoisonTarget() },
                    { EffectType.BurnTarget, new PoisonTarget() },
                    { EffectType.ParalyseTarget, new PoisonTarget() },
                    { EffectType.FreezeTarget, new PoisonTarget() },
                    { EffectType.LowerTargetsAttack, new LowerTargetsAttack() },
                    { EffectType.LowerTargetsDefence, new LowerTargetsDefence() },
                    { EffectType.LowerTargetsSpAttack, new LowerTargetsSpAttack() },
                    { EffectType.LowerTargetsSpDefence, new LowerTargetsSpDefence() },
                    { EffectType.LowerTargetsSpeed, new LowerTargetsSpeed() },
                    { EffectType.RaiseTargetsAttack, new RaiseTargetsAttack() },
                    { EffectType.RaiseTargetsDefence, new RaiseTargetsDefence() },
                    { EffectType.RaiseTargetsSpAttack, new RaiseTargetsSpAttack() },
                    { EffectType.RaiseTargetsSpDefence, new RaiseTargetsSpDefence() },
                    { EffectType.RaiseTargetsSpeed, new RaiseTargetsSpeed() },
                    { EffectType.LowerUsersAttack, new LowerUsersAttack() },
                    { EffectType.LowerUsersDefence, new LowerUsersDefence() },
                    { EffectType.LowerUsersSpAttack, new LowerUsersSpAttack() },
                    { EffectType.LowerUsersSpDefence, new LowerUsersSpDefence() },
                    { EffectType.LowerUsersSpeed, new LowerUsersSpeed() },
                    { EffectType.RaiseUsersAttack, new RaiseUsersAttack() },
                    { EffectType.RaiseUsersDefence, new RaiseUsersDefence() },
                    { EffectType.RaiseUsersSpAttack, new RaiseUsersSpAttack() },
                    { EffectType.RaiseUsersSpDefence, new RaiseUsersSpDefence() },
                    { EffectType.RaiseUsersSpeed, new RaiseUsersSpeed() },
                }
            );
    }
}