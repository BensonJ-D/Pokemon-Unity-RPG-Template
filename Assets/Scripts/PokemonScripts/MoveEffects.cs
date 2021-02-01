using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

namespace PokemonScripts
{
    public enum EffectType { PoisonTarget, LowerTargetsAttackOneStage }

    public interface IMoveEffect
    {
        void ApplyEffect(Pokemon user, Pokemon target);
        void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect2);
    }
    
    public static class MoveEffectLookup {
        public static readonly ReadOnlyDictionary<EffectType, IMoveEffect> MoveEffectFunctions =
            new ReadOnlyDictionary<EffectType, IMoveEffect>(
                new Dictionary<EffectType, IMoveEffect>()
                {
                    { EffectType.PoisonTarget, new PoisonTarget() },
                    { EffectType.LowerTargetsAttackOneStage, new LowerTargetsAttackOneStage() }
                }
            );
    }
    
    public class PoisonTarget : IMoveEffect
    {
        public void ApplyEffect(Pokemon user, Pokemon target) { }
        public void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect) { }
    }
    
    public class ModifyStat : IMoveEffect
    {
        public virtual void ApplyEffect(Pokemon user, Pokemon target) { Debug.Log("Function called"); }
        public virtual void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect2)
        {
            target.StatBoosts[(Stat) effect1] = 
                Mathf.Clamp(target.StatBoosts[(Stat) effect1] + effect2, -6, 6);
            Debug.Log("Base effect called");
        }
    }
    
    public class LowerTargetsAttackOneStage : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            Debug.Log("Child effect called");
            base.ApplyEffect(user, target, (int) Stat.Attack, -1);
        }
    }
    
    public class LowerTargetsAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            Debug.Log("Child effect called");
            base.ApplyEffect(user, target, (int) Stat.Attack, -1);
        }
    }
}