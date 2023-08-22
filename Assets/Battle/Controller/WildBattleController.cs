using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Window;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using Characters.Player;
using GameSystem.Window.Dialog;
using Menus.ActionMenu;
using Menus.InventoryMenu;
using Menus.MoveMenu;
using Menus.Party;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle.Controller
{
    public class WildBattleController : PlayerBattleController
    {
        public WildBattleController(Player player, ApplyDamageCallback applyDamageCallbackCallback, TextBox textBox)
        {
            Initialise(player, applyDamageCallbackCallback);
            TextBox = textBox;
        }

        public override IEnumerator ChooseActions(List<PokemonCombatant> combatants, List<PokemonCombatant> targets)
        {
            ChosenActions.Clear();

            foreach (var combatant in combatants)
            {
                var pokemon = combatant.Pokemon;
                var moves = pokemon.Moves;
                var moveIndex = Random.Range(0, moves.Count);
                
                var enemies = (from target in targets
                    where target.Team != combatant.Team
                    select target);


                var allies = (from target in targets
                    where target.Team != combatant.Team
                    select target);

                var newAction = new BattleAction
                {
                    Priority = BattleActionPriority.Move,
                    Action = PerformMove(combatant, enemies.ToList(), combatant.Pokemon.Moves[moveIndex]),
                    Combatant = combatant
                };

                ChosenActions.Add(newAction);
                yield return null;
            }
        }
        
        private static IEnumerator DebugAttack(Pokemon pokemon, Move move)
        {
            Debug.Log($"Wild {pokemon.Name} used {move}");
            yield return null;
        }
    }
}