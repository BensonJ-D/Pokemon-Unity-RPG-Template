using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Utilities.LinkedEnumerator;
using System.Window;
using Battle.Controller;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Moves;
using Characters.Players;
using GameSystem.Transition;
using GameSystem.Utilities.Tasks;
using GameSystem.Window.Dialog;
using Menus.ActionMenu;
using Menus.InventoryMenu;
using Menus.MoveMenu;
using Menus.Party;
using MyBox;
using UnityEngine;

namespace Battle
{
    public enum BattleState
    {
        Start, Turn, Battle,
        End
    }

    public enum SubsystemState
    {
        Open, Closed
    }

    public enum TurnState
    {
        Busy, Ready
    }

    public class BattleWindow : WindowBase
    {
        public delegate IEnumerator OnFaintCallback(IEnumerable<PokemonCombatant> faintedCombatants);

        public delegate void OnSwitchCallback(PokemonCombatant combatant,
            IEnumerable<PokemonCombatant> activeCombatants);

        public delegate void OnSwitchTrigger(PokemonCombatant combatant);

        [Separator("Menus")] [SerializeField] private ActionMenu actionMenu;

        [SerializeField] private MoveMenu moveMenu;
        [SerializeField] private PartyMenu partyMenu;
        [SerializeField] private InventoryMenu inventoryMenu;

        [Separator("Dialog")] [SerializeField] private TextBox textBox;

        [Separator("Combatants")] [SerializeField]
        private List<PokemonCombatant> combatants;

        private List<BattleAction> _actions;
        private LinkedEnumerator<BattleAction> _actionsEnumerator;
        private bool _isWildBattle;
        private Player _localPlayer;
        private List<PlayerBattleController> _participants;

        private Player _wildPokemon;

        private BattleState BattleState { get; set; } = BattleState.Start;
        private event OnSwitchCallback OnSwitch;
        private event OnFaintCallback OnFaint;

        private Dictionary<Player, PlayerBattleController> GetBattleControllers(List<Player> participants) {
            return (
                from participant in participants
                select (participant, controller: GetBattleController(participant))
            ).ToDictionary(pair => pair.participant, pair => pair.controller);
        }

        private PlayerBattleController GetBattleController(Player participant) {
            switch (participant.ControllerType) {
                case ControllerType.Local:
                {
                    var controller = new LocalBattleController(participant, ApplyDamage,
                        actionMenu, moveMenu, partyMenu, inventoryMenu, textBox,
                        ref OnSwitch, OnSwitchFunc, ref OnFaint);
                    _localPlayer = controller;
                    return controller;
                }

                case ControllerType.Wild:
                {
                    var controller = new WildBattleController(participant, ApplyDamage, textBox);
                    _wildPokemon = controller;
                    return controller;
                }

                default:
                    return new WildBattleController(null, null, null);
            }
        }

        public IEnumerator OpenWindow(List<Player> participants, bool isWildBattle) {
            _actions ??= new List<BattleAction>();
            _actions.Clear();
            textBox.ClearText();
            OnSwitch = null;

            var uniqueParticipants = participants.Distinct().ToList();
            var controllersDict = GetBattleControllers(uniqueParticipants);
            _participants = (from controllerPair in controllersDict select controllerPair.Value).ToList();
            _isWildBattle = isWildBattle;

            var playerCombatantPairs = participants.Zip(combatants, (player, combatant) => (player, combatant));
            foreach (var (player, combatant) in playerCombatantPairs) {
                combatant.ControllingPlayer = controllersDict[player];
                var pokemon = player.Party.GetNextBattleReadyPokemon(combatant.Position);
                combatant.Setup(pokemon);
            }

            yield return base.OpenWindow();
        }

        public IEnumerator RunWindow() {
            var pokemonEntryAnimationTasks =
                (from combatant in combatants
                    where combatant.Pokemon != null
                    select new Task(combatant.PlayEnterAnimation()))
                .ToList();

            combatants.ForEach(OnSwitchFunc);

            yield return new WaitWhile(() => TransitionController.TransitionState != TransitionState.None
                                             || pokemonEntryAnimationTasks.Any(task => task.Running));

            if (_isWildBattle)
                yield return textBox.TypeDialog($"{_wildPokemon.Party.PartyMembers.First().Name} appeared!");
            yield return null;
            if (_isWildBattle)
                yield return textBox.TypeDialog("This is a message ending in g and getting a much longer line.");

            BattleState = BattleState.Start;
            while (BattleState != BattleState.End) yield return RunTurn();
        }

        private IEnumerator RunTurn() {
            if (BattleState != BattleState.Start) yield break;

            _actions.Clear();

            BattleState = BattleState.Turn;

            var combatGroups =
                (from combatant in combatants
                    group combatant by combatant.ControllingPlayer
                    into grouping
                    select grouping)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());

            var participantTurnTasks =
                (from participant in _participants
                    select new Task(participant.ChooseActions(combatGroups[participant], combatants)))
                .ToList();

            yield return new WaitWhile(() => participantTurnTasks.Any(task => task.Running));

            var newActions = (from participant in _participants
                select participant.ChosenActions).SelectMany(actionList => actionList);

            BattleState = BattleState.Battle;

            _actions.AddRange(newActions);
            _actions.Sort(PrioritizeActions);

            _actionsEnumerator = new LinkedEnumerator<BattleAction>(_actions);
            while (_actionsEnumerator.MoveNext()) yield return _actionsEnumerator.CurrentValue?.Action;

            if (_participants.Any(participant => !participant.AbleToBattle)) {
                BattleState = BattleState.End;
                yield break;
            }

            BattleState = BattleState.Start;
        }

        // public enum BattleAction { NewPokemon = -3, Weather = -2, PersistentDamage = -1, Move = 0, Item = 1, Switch = 2, Run = 3 }
        private int PrioritizeActions(BattleAction b, BattleAction a) {
            var (action2, participant2) = (b.Priority, b.Combatant);
            var (action1, participant1) = (a.Priority, a.Combatant);
            if (action2 != action1) return action1 - action2;

            var coinFlip = Random.Range(0, 2) == 0 ? -1 : 1;

            if (action1 == BattleActionPriority.Switch || action1 == BattleActionPriority.Item) return coinFlip;

            var pokemon2 = participant2.Pokemon;
            var pokemon1 = participant1.Pokemon;
            var speed2 = pokemon2.BoostedSpeed;
            var speed1 = pokemon1.BoostedSpeed;

            if (speed2 != speed1) return speed1 - speed2;
            return coinFlip;
        }

        private IEnumerator ApplyDamage(PokemonCombatant attacker, List<PokemonCombatant> targets, Move move) {
            var damageDetails = DamageDetails.CalculateDamage(attacker, targets, move);
            var damageTasks = damageDetails.Select(result => new Task(result.Target.ApplyDamage(result))).ToList();
            var lastQueuedMessageTask = damageDetails.Aggregate(Task.EmptyTask,
                (previousTask, result) => previousTask.QueueTask(DisplayDamageText(result)));
            yield return new WaitWhile(() => damageTasks.Any(task => task.Running) || lastQueuedMessageTask.Running);

            var faintedCombatants = combatants.Where(combatant => combatant.Pokemon.IsFainted).ToList();

            if (faintedCombatants.IsNullOrEmpty()) yield break;
            _actionsEnumerator.RemoveNext(action => faintedCombatants.Contains(action.Combatant));

            _actionsEnumerator.InsertNext(new BattleAction {
                Priority = 0,
                Action = OnFaint?.Invoke(faintedCombatants),
                Combatant = null
            });

            faintedCombatants.ForEach(combatant =>
                _actionsEnumerator.InsertNext(new BattleAction {
                    Priority = 0,
                    Action = (combatant.ControllingPlayer as PlayerBattleController)?.PerformFaint(combatant),
                    Combatant = combatant
                })
            );

            _participants.ForEach(participant => {
                if (participant.Party.HasNoBattleReadyPokemon())
                    _actionsEnumerator.Append(new BattleAction {
                        Priority = 0,
                        Action = participant.OnDefeat(),
                        Combatant = null
                    });
            });
        }

        private IEnumerator DisplayDamageText(DamageDetails damageDetails) {
            if (damageDetails.Critical) {
                yield return textBox.TypeMessage("It's a critical hit!", false);
                yield return new WaitForSeconds(1f);
            }

            switch (damageDetails.Effective) {
                case AttackEffectiveness.NoEffect:
                    yield return textBox.TypeMessage("The move had no effect ...", false);
                    yield return new WaitForSeconds(1f);
                    break;
                case AttackEffectiveness.NotVeryEffective:
                    yield return textBox.TypeMessage("It's not very effective ...", false);
                    yield return new WaitForSeconds(1f);
                    break;
                case AttackEffectiveness.SuperEffective:
                    yield return textBox.TypeMessage("It's super effective!", false);
                    yield return new WaitForSeconds(1f);
                    break;
            }
        }

        private void OnSwitchFunc(PokemonCombatant combatant) {
            OnSwitch?.Invoke(combatant, combatants);
        }

        protected override IEnumerator OnClose() {
            OnSwitch = null;
            _participants.ForEach(p => p.Party.ResetBattleOrder());
            yield return base.OnClose();
        }
    }
}