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
        public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
            return base.ApplyEffect(user, target,  Stat.Attack, modifier);
        }
    }
    
    public class ModifyDefence : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
            return base.ApplyEffect(user, target,  Stat.Defence, modifier);
        }
    }

    public class ModifySpAttack : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
            return base.ApplyEffect(user, target, Stat.SpAttack, modifier);
        }
    }

    public class ModifySpDefence : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
            return base.ApplyEffect(user, target,  Stat.SpDefence, modifier);
        }
    }
    
    public class ModifySpeed : ModifyStat
    {
        public override string ApplyEffect(Pokemon user, Pokemon target, object modifier) { 
            return base.ApplyEffect(user, target,  Stat.Speed, modifier);
        }
    }
}