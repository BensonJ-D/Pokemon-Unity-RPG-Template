using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PokemonScripts.Moves.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace PokemonScripts
{
    public interface IMoveEffect
    {
        void ApplyEffect(Pokemon user, Pokemon target);
        void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect2);
    }
    
    public class PoisonTarget : IMoveEffect
    {
        public void ApplyEffect(Pokemon user, Pokemon target) { }
        public void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect) { }
    }
    
    public class ModifyStat : IMoveEffect
    {
        public virtual void ApplyEffect(Pokemon user, Pokemon target) { }
        public virtual void ApplyEffect(Pokemon user, Pokemon target, int effect1, int effect2)
        {
            target.StatBoosts[(Stat) effect1] = 
                Mathf.Clamp(target.StatBoosts[(Stat) effect1] + effect2, -6, 6);
        }
    }
}