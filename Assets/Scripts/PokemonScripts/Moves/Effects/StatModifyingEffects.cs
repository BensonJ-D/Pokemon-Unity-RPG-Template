using System;
using UnityEngine;

namespace PokemonScripts.Moves.Effects
{
    [Serializable]
    public class StatModifierEffect
    {
        [SerializeField] private MoveTarget target;
        [SerializeField] private StatModifiers.EffectType stat;
        [SerializeField] private int modifier;

        public MoveTarget Target => target;
        public StatModifiers.EffectType Stat => stat;
        public int Modifier => modifier;
    }
    
    public class ModifyAttack : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, int modifier) { 
            return base.ApplyEffect(user, target, (int) Stat.Attack, modifier);
        }
    }
    
    public class ModifyDefence : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, int modifier) { 
            return base.ApplyEffect(user, target, (int) Stat.Defence, modifier);
        }
    }

    public class ModifySpAttack : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, int modifier) { 
            return base.ApplyEffect(user, target, (int) Stat.SpAttack, modifier);
        }
    }

    public class ModifySpDefence : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, int modifier) { 
            return base.ApplyEffect(user, target, (int) Stat.SpDefence, modifier);
        }
    }
    
    public class ModifySpeed : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, int modifier) { 
            return base.ApplyEffect(user, target, (int) Stat.Speed, modifier);
        }
    }
}