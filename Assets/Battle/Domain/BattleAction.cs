using System.Collections;
using Characters.Battle.Pokemon;

namespace Battle.Domain
{
    public class BattleAction
    {
        public BattleActionPriority Priority;
        public IEnumerator Action;
        public PokemonCombatant Combatant;
    }
}