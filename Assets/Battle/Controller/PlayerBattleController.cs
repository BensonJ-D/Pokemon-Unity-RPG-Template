using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Utilities.Tasks;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using Characters.Player;
using GameSystem.Window.Dialog;
using UnityEngine;

namespace Battle.Controller
{
    public enum ControllerPosition { Player1, Player2, Player3, Player4 }
    public enum ControllerType { Local, Remote, AI, Wild }

    public abstract class PlayerBattleController : Player
    {
        protected TextBox TextBox { get; set; }
        protected Task TextTask;
        protected BattleWindow.OnSwitchTrigger OnSwitch;

        public void Initialise(Player player, ApplyDamageCallback applyDamageCallbackCallback) {
            name = player?.Name;
            party = player?.Party;
            inventory = player?.Inventory;
            controllerType = player?.ControllerType ?? ControllerType.Wild;
            _applyDamageCallback = applyDamageCallbackCallback;
            
            ChosenActions = new List<BattleAction>();
        }

        public PlayerBattleController() : this(null, null) { }
        public PlayerBattleController(Player player, ApplyDamageCallback applyDamageCallbackCallback) 
            { Initialise(player, applyDamageCallbackCallback); }
        
        public delegate IEnumerator ApplyDamageCallback(PokemonCombatant attacker, List<PokemonCombatant> targets, Move move);
        private ApplyDamageCallback _applyDamageCallback;
        
        private bool _ableToBattle = true;
        public bool AbleToBattle => _ableToBattle;
        public List<BattleAction> ChosenActions { get; protected set; }
        public abstract IEnumerator ChooseActions(List<PokemonCombatant> combatants, List<PokemonCombatant> targets);
        
        public IEnumerator OnDefeat() {
            _ableToBattle = false;
            yield break;
        } 
        
        protected IEnumerator PerformMove(PokemonCombatant attacker, List<PokemonCombatant> targets, Move move) {
            yield return TextBox.TypeMessage($"{attacker.Pokemon.Base.Species} used {move.Base.Name}!");
            yield return attacker.PlayBasicHitAnimation();

            var targetAnimationTasks = targets.Select(target => new Task(target.PlayDamageAnimation())).ToList();
            yield return new WaitWhile(() => targetAnimationTasks.Any(task => task.Running));
            
            var applyDamage = new Task(_applyDamageCallback(attacker, targets, move));
            yield return new WaitWhile(() => applyDamage.Running);
        }
        
        protected IEnumerator PerformSwitch(PokemonCombatant activeCombatant, int targetPokemonIndex) {
            yield return PerformSwitchOut(activeCombatant);
            yield return PerformSwitchIn(activeCombatant, targetPokemonIndex);
        }
        
        public IEnumerator PerformFaint(PokemonCombatant combatant) {
            yield return TextBox.TypeMessage($"{combatant.Pokemon.Name} fainted!");
            yield return combatant.PlayFaintAnimation();
        }
        
        protected IEnumerator PerformSwitchOut(PokemonCombatant activeCombatant) {
            yield return TextBox.TypeMessage($"Good job, {activeCombatant.Pokemon.Name}!");
            yield return activeCombatant.PlayFaintAnimation();
        }
        
        private IEnumerator PerformSwitchIn(PokemonCombatant activeCombatant, int targetPokemonIndex) {
            party.SwapBattlePokemonPositions(activeCombatant.Position, targetPokemonIndex);
            var newPokemon = party.GetBattleOrderedPokemon(activeCombatant.Position);
            activeCombatant.Setup(newPokemon);

            OnSwitch.Invoke(activeCombatant);
            
            Task enterAnimation = new Task(activeCombatant.PlayEnterAnimation());
            TextTask = new Task(TextBox.TypeMessage($"Let's go, {activeCombatant.Pokemon.Name}!!"));
            yield return new WaitWhile(() => enterAnimation.Running || TextTask.Running);
            yield return new WaitForSeconds(1f);
        }

        protected virtual void OnPokemonSwitch(PokemonCombatant switchedCombatant, IEnumerable<PokemonCombatant> activeCombatants) { }

        protected virtual IEnumerator OnPokemonFaint(IEnumerable<PokemonCombatant> faintedCombatants) { yield break; }
    }
}