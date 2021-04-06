using System.Collections.Generic;
using PokemonScripts.Moves.Effects;

namespace PokemonScripts.Moves
{
    public class Move
    {
        public MoveBase Base { get; set; }
        public int Pp { get; set; }

        public Move(MoveBase pBase)
        {
            Base = pBase;
            Pp = pBase.Pp;
        }

        public List<string> ApplyEffects(Pokemon user, Pokemon opponent)
        {
            List<string> messages = new List<string>();
            foreach (var statModifier in Base.StatModifierEffects)
            {
                var target = statModifier.Target == MoveTarget.Self ? user : opponent;
                var message = StatModifierEffects.GetEffectClass[statModifier.Stat]
                    .ApplyEffect(user, target, (int) statModifier.Stat, statModifier.Modifier);
                if(message.Length > 0) { messages.Add(message); }
            }

            foreach (var primaryCondition in Base.PrimaryStatusEffects)
            {
                var target = Base.Target == MoveTarget.Self ? user : opponent;
                var message = PrimaryStatusEffects.GetEffectClass[primaryCondition.StatusCondition]
                    .ApplyEffect(user, target);
                if(message.Length > 0) { messages.Add(message); }
            }

            return messages;
        }
    }
}