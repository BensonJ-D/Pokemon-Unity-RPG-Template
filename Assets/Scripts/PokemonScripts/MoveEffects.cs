using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

namespace PokemonScripts
{
    public enum EffectType { PoisonTarget, LowerTargetsAttack }

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
                    { EffectType.LowerTargetsAttack, new LowerTargetsAttack() }
                }
            );
    }
    
    public class PoisonTarget : IMoveEffect
    {
        public virtual void ApplyEffect(Pokemon user, Pokemon target) { }
        public virtual void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect) { }
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
    
    public class LowerTargetsAttack : ModifyStat
    {
        public override void ApplyEffect(Pokemon user, Pokemon target) { 
            Debug.Log("Child effect called");
            base.ApplyEffect(user, target, (int) Stat.Attack, -1);
        }
    }
}