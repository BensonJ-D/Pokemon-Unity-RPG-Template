using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transition;
using System.Utilities.Input;
using System.Utilities.LinkedEnumerator;
using System.Utilities.Tasks;
using System.Window;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using Characters.Player;
using Cinemachine;
using GameSystem.Window.Dialog;
using Menus.ActionMenu;
using Menus.InventoryMenu;
using Menus.MoveMenu;
using Menus.Party;
using Menus.Party.MenuItem;
using Menus.PartyMenu.MenuItem;
using MyBox;
using Popup;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
        private Task _inventoryMenuTask;

        private GameObject _levelUpWindowPrefab = Resources.Load("Prefabs/LevelUpWindow") as GameObject;

        private Dictionary<Pokemon, HashSet<Pokemon>> _pokemonObserved;

        public LocalBattleController(Player player, ApplyDamageCallback applyDamageCallbackCallback, ActionMenu actionMenu, MoveMenu moveMenu, PartyMenu partyMenu,
            InventoryMenu inventoryMenu, TextBox textBox, 
            ref BattleWindow.OnSwitchCallback onSwitch, BattleWindow.OnSwitchTrigger onSwitchTriggerOnSwitch,
            ref BattleWindow.OnFaintCallback onFaint) {
            Initialise(player, applyDamageCallbackCallback);
            
            ActionMenu = actionMenu;
            MoveMenu = moveMenu;
            PartyMenu = partyMenu;
            InventoryMenu = inventoryMenu;
            TextBox = textBox;

            onSwitch += OnPokemonSwitch;
            onFaint += OnPokemonFaint;
            
            OnSwitch = onSwitchTriggerOnSwitch;
            
            _pokemonObserved = new Dictionary<Pokemon, HashSet<Pokemon>>();
            Party.PartyMembers.ForEach(pokemon => _pokemonObserved.Add(pokemon, new HashSet<Pokemon>()));
        }
        
        public override IEnumerator ChooseActions(List<PokemonCombatant> combatants, List<PokemonCombatant> targets) {
            ChosenActions.Clear();

            var participantControlledCombatants = new LinkedEnumerator<PokemonCombatant>(combatants);
            
            participantControlledCombatants.MoveNext();
            while (participantControlledCombatants.CurrentValue != null) {
                var isCancellable = participantControlledCombatants.HasPrevious();
                var combatant = participantControlledCombatants.CurrentValue;
            
                yield return ChooseAction(combatant, targets, isCancellable);
                if (ActionMenu.CloseReason == WindowCloseReason.Cancel)
                {
                    participantControlledCombatants.MovePrevious();
                    continue;
                }

                if(_actionChosen) participantControlledCombatants.MoveNext();
            }
        }

        private IEnumerator ChooseAction(PokemonCombatant combatant, List<PokemonCombatant> targets, bool isCancellable) {
            _actionChosen = false;
            TextTask = new Task(TextBox.TypeMessage($"What will {combatant.Pokemon.Name} do?",
                20f, false));

            if (_actionMenuTask == null || !_actionMenuTask.Running) {
                yield return ActionMenu.OpenWindow(isCancellable);
                _actionMenuTask = new Task(ActionMenu.RunWindow());
            }

            if (_actionMenuTask.Paused) {
                ActionMenu.Reset();
                _actionMenuTask.Unpause();
            }

            yield return new WaitWhile(() => ActionMenu.CloseReason == null);
            
            switch (ActionMenu.Choice.Value) {
                case ActionMenuOption.Fight:
                    yield return ChooseMove(combatant, targets);
                    break;
                case ActionMenuOption.Bag:
                    yield return ChooseItem();
                    break;
                case ActionMenuOption.Pokemon:
                    yield return ChoosePokemon(combatant);
                    break;
                case ActionMenuOption.Run:
                    _actionMenuTask.Stop();
                    yield return ActionMenu.CloseWindow();
                    // OnBattleOver?.Invoke(false);
                    break;
            }
        }
        
        private IEnumerator ChooseMove(PokemonCombatant currentCombatant, List<PokemonCombatant> targets) {
            if (TextTask?.Running == true) TextTask.Stop(); 
            TextBox.ClearText();
            
            if (_actionMenuTask?.Running == true) _actionMenuTask.Stop();
            yield return ActionMenu.CloseWindow();

            yield return MoveMenu.OpenWindow(currentCombatant.Pokemon.Moves);
            yield return MoveMenu.RunWindow();

            if (MoveMenu.CloseReason == WindowCloseReason.Cancel) yield break;

            var enemies = (from target in targets
                where target.Team != currentCombatant.Team
                select target);


            var allies = (from target in targets
                where target.Team == currentCombatant.Team && target != currentCombatant
                select target);

            var pokemon = currentCombatant.Pokemon;
            var newAction = new BattleAction {
                Priority = BattleActionPriority.Move,
                Action = PerformMove(currentCombatant, enemies.ToList(), MoveMenu.Choice.Value),
                Combatant = currentCombatant
            };

            ChosenActions.Add(newAction);
            _actionChosen = true;
        }
        
                
        private IEnumerator ChoosePokemon(PokemonCombatant combatant) {
            _actionMenuTask.Pause();
            
            var task = new Task(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return OpenPartyMenu();
            yield return TransitionController.WaitForTransitionCompletion;

            _partyMenuTask = new Task(PartyMenu.RunWindow());
            yield return new WaitWhile(() => PartyMenu.CloseReason == null);
            
            if (PartyMenu.CloseReason == WindowCloseReason.Cancel) yield break;

            var pokemonIndex = ((PartyMenuItem) PartyMenu.Choice).PositionInOrder;
            var newAction = new BattleAction {
                Priority = BattleActionPriority.Switch,
                Action = PerformSwitch(combatant, pokemonIndex),
                Combatant = combatant
            };

            ChosenActions.Add(newAction);
            _actionChosen = true;

            _actionMenuTask?.Stop();
            TextTask?.Stop();
            TextBox.ClearText();
            
            task = new Task(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return PartyMenu.CloseWindow();
            yield return ActionMenu.CloseWindow();
            yield return TransitionController.WaitForTransitionCompletion;
        }

        private IEnumerator OpenPartyMenu() {
            TextTask.Stop();
            TextBox.ClearText();

            _partyMenuTask?.Stop();

            yield return PartyMenu.OpenWindow(Party, isBattle: true, onCancelCallback: OnPartyCancel);
        }
        
        private IEnumerator OnPartyCancel() {
            var task = new Task(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return PartyMenu.CloseWindow();
            yield return TransitionController.WaitForTransitionCompletion;
        }
        
        private IEnumerator ChooseItem() {
            _actionMenuTask.Pause();
            
            var task = new Task(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return InventoryMenu.OpenWindow(Inventory, Party, onCancelCallback: OnInventoryCancel);
            yield return TransitionController.WaitForTransitionCompletion;
            
            TextTask.Stop();
            TextBox.ClearText();
            
            _inventoryMenuTask = new Task(InventoryMenu.RunWindow());
            yield return new WaitWhile(() => InventoryMenu.CloseReason == null);
        }
        
        private IEnumerator OnInventoryCancel() {
            var task = new Task(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return InventoryMenu.CloseWindow();
            yield return TransitionController.WaitForTransitionCompletion;
        }

        protected override void OnPokemonSwitch(PokemonCombatant combatant, IEnumerable<PokemonCombatant> activeCombatants) {
            if (combatant.ControllingPlayer != this) {
                activeCombatants.Where(c => c.ControllingPlayer == this)
                    .Select(c => c.Pokemon)
                    .ForEach(p => _pokemonObserved[p].Add(combatant.Pokemon));
            }
            else {
                activeCombatants.Where(c => c.ControllingPlayer != this)
                    .Select(c => c.Pokemon)
                    .ForEach(p => _pokemonObserved[combatant.Pokemon].Add(p));
            }
        }
        
        protected override IEnumerator OnPokemonFaint(IEnumerable<PokemonCombatant> faintedCombatants) {
            var pokemonExpGains = new Dictionary<Pokemon, int>();
            var pokemonLevels = new Dictionary<Pokemon, int>();
            Party.GetCurrentBattleOrder().ForEach(pokemon => {
                pokemonExpGains.Add(pokemon, 0);
                pokemonLevels.Add(pokemon, pokemon.Level);
            });

            faintedCombatants.ForEach(c => OnPokemonFaint(c, pokemonExpGains));

            var nonZeroExpGains = pokemonExpGains
                .Where(pair => pair.Value > 0)
                .ToList();
            
            if (nonZeroExpGains.IsNullOrEmpty()) yield break;
            
            var allNames = nonZeroExpGains
                .Select(pair => pair.Key.Name);

            var exp = nonZeroExpGains.First().Value;
            var namesString = string.Join(", ", allNames);
            yield return TextBox.TypeMessage($"{namesString} gained {exp} exp!");
            
            var tasks = nonZeroExpGains.Select(pair => new Task(pair.Key.UpdateExp(pair.Value))).ToList();
            yield return new WaitWhile(() => tasks.Any(t => t.Running));

            var leveledPokemon = pokemonLevels.Where(pair => pair.Key.Level != pair.Value);
            var levelQueue = leveledPokemon.Aggregate(Task.EmptyTask, 
                (previousTask, pl) => previousTask.QueueTask(OnLevelUp(pl.Key, pl.Value)));

            yield return new WaitWhile(() => levelQueue.Running);
        }

        private void OnPokemonFaint(PokemonCombatant faintedCombatant, IDictionary<Pokemon, int> pokemonExpGains) {
            var experienceYield = faintedCombatant.Pokemon.ExperienceYield;
            var activePokemon = _pokemonObserved.Where(pair => pair.Value.Contains(faintedCombatant.Pokemon))
                .Select(p => p.Key).ToList();

            if (activePokemon.IsNullOrEmpty()) return;
            activePokemon.ForEach(p => pokemonExpGains[p] += experienceYield);
        }

        private IEnumerator OnLevelUp(Pokemon pokemon, int originalLevel) {
            yield return TextBox.TypeDialog($"{pokemon.Name} leveled up!");
            yield return InputController.WaitForConfirm;

            var levelUpWindow = Object.Instantiate(_levelUpWindowPrefab);
            if (levelUpWindow != null) {
                yield return levelUpWindow.GetComponent<LevelUpWindow>()
                    .ShowWindow(pokemon.GetStats(originalLevel), pokemon.GetStats(pokemon.Level));
                Object.Destroy(levelUpWindow.gameObject);
            }
            else {
                Object.Destroy(levelUpWindow);
            }
        }
    }
}