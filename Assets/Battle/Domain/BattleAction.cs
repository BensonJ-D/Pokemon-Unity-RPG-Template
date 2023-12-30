using System.Collections;
using Characters.Battle.Pokemon;

namespace Battle.Domain
{
    public class BattleAction
    {
        public IEnumerator Action;
        public PokemonCombatant Combatant;
        public BattleActionPriority Priority;
    }
}