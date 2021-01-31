using System.Collections.Generic;

namespace PokemonScripts
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
            var target = Base.Target == MoveTarget.Self ? user : opponent;
            List<string> messages = new List<string>();
            foreach (var effect in Base.Effects)
            {
                var func = MoveEffectLookup.MoveEffectFunctions[effect];
                func.ApplyEffect(user, target);
            }

            return messages;
        }
    }
}