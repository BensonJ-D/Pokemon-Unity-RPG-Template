using System.Collections.Generic;
using System.Linq;
using Battle;
using PokemonScripts.Moves.Effects;

namespace PokemonScripts.Moves
{
    public class Move
    {
        public MoveBase Base { get; }
        public int Pp { get; }

        public Move(MoveBase pBase)
        {
            Base = pBase;
            Pp = pBase.Pp;
        }

        public IEnumerable<string> ApplyEffects(BattlePokemon user, BattlePokemon opponent)
        {
            List<string> messages = (from statModifier in Base.StatModifierEffects
                let target = statModifier.Target == MoveTarget.Self ? user : opponent
                select StatModifierEffects.GetEffectClass[statModifier.Stat]
                    .ApplyEffect(user, target, (int) statModifier.Stat, statModifier.Modifier)
                into message
                where message.Length > 0
                select message).ToList();
            
            messages.AddRange(from primaryCondition in Base.PrimaryStatusEffects
                let target = Base.Target == MoveTarget.Self ? user : opponent
                select PrimaryStatusEffects.GetEffectClass[primaryCondition.StatusCondition]
                    .ApplyEffect(user, target)
                into message
                where message.Length > 0
                select message);

            return messages;
        }
    }
}