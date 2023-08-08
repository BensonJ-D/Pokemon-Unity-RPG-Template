using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transition;
using System.Utilities.DoubleLinkedList;
using System.Utilities.Tasks;
using System.Window;
using System.Window.Dialog;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using Characters.Player;
using Menus.ActionMenu;
using Menus.InventoryMenu;
using Menus.MoveMenu;
using Menus.Party;
using Menus.Party.MenuItem;
using UnityEngine;

namespace Battle.Controller
{
    public class LocalBattleController : PlayerBattleController
    {
        private bool _actionChosen;
        
        private ActionMenu ActionMenu { get; set; }
        private MoveMenu MoveMenu { get; set; }
        private PartyMenu PartyMenu { get; set; }
        private InventoryMenu InventoryMenu { get; set; }
        private Task _actionMenuTask;
        private Task _partyMenuTask;

        public LocalBattleController(Player player, ActionMenu actionMenu, MoveMenu moveMenu, PartyMenu partyMenu,
            InventoryMenu inventoryMenu, TextBox textBox)
        {
            Initialise(player);
            
            ActionMenu = actionMenu;
            MoveMenu = moveMenu;
            PartyMenu = partyMenu;
            InventoryMenu = inventoryMenu;
            TextBox = textBox;
        }
        
        public override IEnumerator ChooseActions(List<PokemonCombatant> combatants, List<PokemonCombatant> targets)
        {
            ChosenActions.Clear();

            var participantControlledCombatants = new LinkedEnumerator<PokemonCombatant>(combatants);
            
            participantControlledCombatants.MoveNext();
            while (participantControlledCombatants.Current != null)
            {
                var isCancellable = participantControlledCombatants.HasPrevious();
                var combatant = participantControlledCombatants.Current;
            
                yield return ChooseAction(combatant, targets, isCancellable);
                if (ActionMenu.CloseReason == WindowCloseReason.Cancel)
                {
                    participantControlledCombatants.MovePrevious();
                    continue;
                }

                if(_actionChosen) participantControlledCombatants.MoveNext();
            }
        }

        private IEnumerator ChooseAction(PokemonCombatant combatant, List<PokemonCombatant> targets, bool isCancellable)
        {
            _actionChosen = false;
            TextTask = new Task(TextBox.TypeDialog($"What will {combatant.Pokemon.Name} do?",
                20f));

            if (_actionMenuTask == null || !_actionMenuTask.Running)
            {
                yield return ActionMenu.OpenWindow(isCancellable);
                _actionMenuTask = new Task(ActionMenu.RunWindow());
            }

            if (_actionMenuTask.Paused)
            {
                ActionMenu.Reset();
                _actionMenuTask.Unpause();
            }

            yield return new WaitWhile(() => ActionMenu.CloseReason == null);
            
            switch (ActionMenu.Choice.Value)
            {
                case ActionMenuOption.Fight:
                    yield return ChooseMove(combatant, targets);
                    break;
                case ActionMenuOption.Bag:
                    yield return ChooseItem();
                    // StartCoroutine(ChooseItem(participant));
                    break;
                case ActionMenuOption.Pokemon:
                    yield return ChoosePokemon(combatant);
                    // StartCoroutine(ChoosePokemon(participant));
                    break;
                case ActionMenuOption.Run:
                    _actionMenuTask.Stop();
                    yield return ActionMenu.CloseWindow();
                    // OnBattleOver?.Invoke(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private IEnumerator ChooseMove(PokemonCombatant currentCombatant, List<PokemonCombatant> targets)
        {
            if (_actionMenuTask?.Running == true) _actionMenuTask.Stop();
            yield return ActionMenu.CloseWindow();
            
            if (TextTask?.Running == true) TextTask.Stop(); 
            TextBox.ClearText();
            
            yield return MoveMenu.OpenWindow(currentCombatant.Pokemon.Moves);
            yield return MoveMenu.RunWindow();

            if (MoveMenu.CloseReason == WindowCloseReason.Cancel) yield break;

            var enemies = (from target in targets
                where target.Team != currentCombatant.Team
                select target);


            var allies = (from target in targets
                where target.Team != currentCombatant.Team
                select target);

            var pokemon = currentCombatant.Pokemon;
            var newAction = new BattleAction
            {
                Priority = BattleActionPriority.Move,
                Action = PerformMove(currentCombatant, enemies.First(), MoveMenu.Choice.Value),
                Combatant = currentCombatant
            };

            ChosenActions.Add(newAction);
            _actionChosen = true;
        }
        
                
        private IEnumerator ChoosePokemon(PokemonCombatant combatant)
        {
            _actionMenuTask.Pause();
            
            var task = new Task(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return OpenPartyMenu();
            yield return TransitionController.Instance.WaitForTransitionCompletion();

            _partyMenuTask = new Task(PartyMenu.RunWindow());
            yield return new WaitWhile(() => PartyMenu.CloseReason == null);
            
            if (PartyMenu.CloseReason == WindowCloseReason.Cancel) yield break;

            var pokemonIndex = ((PartyMenuItem) PartyMenu.Choice).PositionInOrder;
            var newAction = new BattleAction
            {
                Priority = BattleActionPriority.Switch,
                Action = PerformSwitch(combatant, pokemonIndex),
                Combatant = combatant
            };

            ChosenActions.Add(newAction);
            _actionChosen = true;

            _actionMenuTask?.Stop();
            TextTask?.Stop();
            TextBox.ClearText();
            
            task = new Task(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return PartyMenu.CloseWindow();
            yield return ActionMenu.CloseWindow();
            yield return TransitionController.Instance.WaitForTransitionCompletion();
        }

        private IEnumerator OpenPartyMenu()
        {
            TextTask.Stop();
            TextBox.ClearText();

            _partyMenuTask?.Stop();

            yield return PartyMenu.OpenWindow(Party, isBattle: true, onCancelCallback: OnPartyCancel);
        }
        
        private IEnumerator OnPartyCancel()
        {
            var task = new Task(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return PartyMenu.CloseWindow();
            yield return TransitionController.Instance.WaitForTransitionCompletion();
        }
        
        private IEnumerator ChooseItem()
        {
            _actionMenuTask.Pause();
            
            var task = new Task(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return InventoryMenu.OpenWindow(Inventory, Party, onCancelCallback: OnInventoryCancel);
            yield return TransitionController.Instance.WaitForTransitionCompletion();
            
            TextTask.Stop();
            TextBox.ClearText();
            
            yield return InventoryMenu.RunWindow();
            
            
            // if (partyMenu.Choice[participant] == PartyMenu.PokemonChoice.Back)
            // {
            //     StartCoroutine(ChooseAction(participant));
            // }
            // else
            // {
            //     _actions.Add((BattleAction.Switch, PerformSwitch(participant), participant));
            //     _turnState[participant] = TurnState.Ready;
            // }
        }
        
        private IEnumerator OnInventoryCancel()
        {
            var task = new Task(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return InventoryMenu.CloseWindow();
            yield return TransitionController.Instance.WaitForTransitionCompletion();
        }
    }
}