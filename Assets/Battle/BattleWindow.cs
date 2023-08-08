using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transition;
using System.Utilities.Tasks;
using System.Window;
using System.Window.Dialog;
using Battle.Controller;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Player;
using Menus.ActionMenu;
using Menus.InventoryMenu;
using Menus.MoveMenu;
using Menus.Party;
using MyBox;
using UnityEngine;

namespace Battle
{
    public enum BattleState { Start, Turn, Battle, End }
    public enum SubsystemState { Open, Closed }
    public enum TurnState { Busy, Ready }

    public class BattleWindow : WindowBase
    {
        [Separator("Menus")] 
        [SerializeField] private ActionMenu actionMenu;
        [SerializeField] private MoveMenu moveMenu;
        [SerializeField] private PartyMenu partyMenu;
        [SerializeField] private InventoryMenu inventoryMenu;
        
        [Separator("Dialog")] 
        [SerializeField] private TextBox textBox;
        
        [Separator("Combatants")]
        [SerializeField] private List<PokemonCombatant> combatants;

        private BattleState BattleState { get; set; } = BattleState.Start;
        private bool _isWildBattle;

        private Player _wildPokemon;
        private List<PlayerBattleController> _participants;
        private List<BattleAction> _actions;

        private Dictionary<Player, PlayerBattleController> GetBattleControllers(List<Player> participants)
        {
            return (
                from participant in participants
                select (participant, controller: GetBattleController(participant))
            ).ToDictionary(pair => pair.participant, pair => pair.controller);
        }

        private PlayerBattleController GetBattleController(Player participant)
        {
            switch (participant.ControllerType)
            {
                case ControllerType.Local:
                    return new LocalBattleController(participant, actionMenu, moveMenu, partyMenu, inventoryMenu, textBox);
                
                case ControllerType.Wild:
                    return new WildBattleController(participant, textBox);
                
                default:
                    return new WildBattleController(null, null);
            }
        }
        
        public IEnumerator OpenWindow(List<Player> participants, bool isWildBattle)
        {
            _actions ??= new List<BattleAction>();
            _actions.Clear();
            textBox.ClearText();

            var uniqueParticipants = participants.Distinct().ToList();
            var controllersDict = GetBattleControllers(uniqueParticipants);
            _participants = (from controllerPair in controllersDict select controllerPair.Value).ToList();
            _isWildBattle = isWildBattle;
            
            var playerCombatantPairs = participants.Zip(combatants, (player, combatant) => (player, combatant));
            foreach (var (player, combatant) in playerCombatantPairs)
            {
                combatant.ControllingPlayer = controllersDict[player];
                var pokemon = player.Party.GetNextBattleReadyPokemon(combatant.Position);
                combatant.Setup(pokemon);
            }

            if (_isWildBattle)
                _wildPokemon = uniqueParticipants.First(player => player.ControllerType == ControllerType.Wild);

            yield return base.OpenWindow();
        }

        public IEnumerator RunWindow()
        {
            var pokemonEntryAnimationTasks =
                (from combatant in combatants
                where combatant.Pokemon != null 
                select new Task(combatant.PlayEnterAnimation()))
                .ToList();

            yield return new WaitWhile(() => TransitionController.TransitionState != TransitionState.None
                                             || pokemonEntryAnimationTasks.Any(task => task.Running));
            
            if(_isWildBattle) yield return textBox.TypeDialog($"{_wildPokemon.Name} appeared!");
            yield return new WaitForSeconds(1f);
            
            BattleState = BattleState.Start;
            yield return RunTurn();
        }

        private IEnumerator RunTurn()
        {
            if (BattleState != BattleState.Start) yield break;
        
            BattleState = BattleState.Turn;

            var combatGroups =
                (from combatant in combatants
                 group combatant by combatant.ControllingPlayer
                 into grouping select grouping)
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
            
            var actionEnumerator = _actions.GetEnumerator();
            while(actionEnumerator.MoveNext())
            {
                yield return actionEnumerator.Current.Action;
            }
            actionEnumerator.Dispose();

            BattleState = BattleState.Start;
            
            StartCoroutine(RunTurn());
        }

        // public enum BattleAction { NewPokemon = -3, Weather = -2, PersistentDamage = -1, Move = 0, Item = 1, Switch = 2, Run = 3 }
        private int PrioritizeActions(BattleAction b, BattleAction a)
        {
            var (action2, participant2) = (b.Priority, b.Combatant);
            var (action1, participant1) = (a.Priority, a.Combatant);
            if (action2 != action1) { return action1 - action2; }
            
            var coinFlip = Random.Range(0, 2) == 0 ? -1 : 1;
            
            if (action1 == BattleActionPriority.Switch || action1 == BattleActionPriority.Item) { return coinFlip; }
        
            var pokemon2 = participant2.Pokemon;
            var pokemon1 = participant1.Pokemon;
            var speed2 = pokemon2.BoostedSpeed;
            var speed1 = pokemon1.BoostedSpeed;
        
            if(speed2 != speed1) { return speed1 - speed2; }
            return coinFlip;
        }
    }
}