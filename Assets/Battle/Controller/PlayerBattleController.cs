using System;
using System.Collections;
using System.Collections.Generic;
using System.Utilities.Tasks;
using System.Window.Dialog;
using Battle.Domain;
using Characters.Battle.Pokemon;
using Characters.Monsters;
using Characters.Moves;
using Characters.Player;
using UnityEngine;

namespace Battle.Controller
{
    public enum ControllerPosition { Player1, Player2, Player3, Player4 }
    public enum ControllerType { Local, Remote, AI, Wild }

    public abstract class PlayerBattleController : Player
    {
        protected TextBox TextBox { get; set; }
        protected Task TextTask;
        
        public void Initialise(Player player)
        {
            name = player?.Name;
            party = player?.Party;
            inventory = player?.Inventory;
            controllerType = player?.ControllerType ?? ControllerType.Wild;
            
            ChosenActions = new List<BattleAction>();
        }

        public PlayerBattleController() : this(null) { }
        public PlayerBattleController(Player player) { Initialise(player); }
        
        public List<BattleAction> ChosenActions { get; protected set; }
        public abstract IEnumerator ChooseActions(List<PokemonCombatant> combatants, List<PokemonCombatant> targets);
        
        protected IEnumerator PerformMove(PokemonCombatant attacker, PokemonCombatant defender, Move move)
        {
            var damageDetails = DamageDetails.CalculateDamage(attacker.Pokemon, defender.Pokemon, move);
            yield return TextBox.TypeDialog($"{attacker.Pokemon.Base.Species} used {move.Base.Name}!");
            yield return attacker.PlayBasicHitAnimation();
            yield return defender.PlayDamageAnimation();
            
            Task applyDamage = new Task(defender.ApplyDamage(damageDetails));
            Task displayDamageMessages = new Task(DisplayDamageText(damageDetails));
            yield return new WaitWhile(() => applyDamage.Running || displayDamageMessages.Running);
        }
        
        protected IEnumerator PerformSwitch(PokemonCombatant activeCombatant, int targetPokemonIndex)
        {
            yield return TextBox.TypeDialog($"Good job, {activeCombatant.Pokemon.Name}!");
            yield return activeCombatant.PlayFaintAnimation();
            yield return PerformSwitchIn(activeCombatant, targetPokemonIndex);
        }
        
        private IEnumerator PerformSwitchIn(PokemonCombatant activeCombatant, int targetPokemonIndex)
        {
            party.SwapBattlePokemonPositions(activeCombatant.Position, targetPokemonIndex);
            var newPokemon = party.GetBattleOrderedPokemon(activeCombatant.Position);
            activeCombatant.Setup(newPokemon);
            
            Task enterAnimation = new Task(activeCombatant.PlayEnterAnimation());
            TextTask = new Task(TextBox.TypeDialog($"Let's go, {activeCombatant.Pokemon.Name}!!"));
            yield return new WaitWhile(() => enterAnimation.Running || TextTask.Running);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator DisplayDamageText(DamageDetails damageDetails)
        {
            if (damageDetails.Critical)
            {
                yield return TextBox.TypeDialog("It's a critical hit!", false);
                yield return new WaitForSeconds(1f);
            }

            switch (damageDetails.Effective)
            {
                case AttackEffectiveness.NoEffect:
                    yield return TextBox.TypeDialog("The move had no effect ...", false);
                    yield return new WaitForSeconds(1f);
                    break;
                case AttackEffectiveness.NotVeryEffective:
                    yield return TextBox.TypeDialog("It's not very effective ...", false);
                    yield return new WaitForSeconds(1f);
                    break;
                case AttackEffectiveness.SuperEffective:
                    yield return TextBox.TypeDialog("It's super effective!", false);
                    yield return new WaitForSeconds(1f);
                    break;
            }
        }
    }
}