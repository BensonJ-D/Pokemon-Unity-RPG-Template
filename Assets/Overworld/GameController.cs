using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Characters.Monsters;
using Characters.Players;
using GameSystem.Transition;
using GameSystem.Utilities.Tasks;
using Overworld.Encounters;
using Overworld.Players;
using UnityEngine;

namespace Overworld {
    public enum GameState { Start, Moving, Talking, Battle, Menu }

    public class GameController : MonoBehaviour
    {
        [SerializeField] private BattleWindow battleWindow;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private TransitionController transitionController;
    
        public static GameState GameState { get; private set; } = GameState.Moving;
        private static class MenuOptions
        {
            public const string 
                Switch = "SWITCH",
                Summary = "SUMMARY",
                Cancel = "CANCEL";
        }

        private Task _playerMovement;
        private Task _openMenu;
    
        private void Start()
        {
            EncounterRegion[] encounterRegions = FindObjectsOfType<EncounterRegion>();

            foreach (var region in encounterRegions)
            {
                region.OnEncountered += StartWildBattle;
            }
        }

        private void Update() {
            switch (GameState) {
                case GameState.Moving:
                {
                    // if (Input.GetKeyDown(KeyCode.Q)) { _openMenu = new Task(OpenMenu()); }

                    var isNotMoving = _playerMovement is null || !_playerMovement.Running;
                    if (isNotMoving) {
                        _playerMovement = new Task(playerController.HandleMovement());
                    }

                    break;
                }
                case GameState.Battle:
                    break;
                case GameState.Start:
                    break;
                case GameState.Talking:
                    break;
                case GameState.Menu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private IEnumerator StartWildBattle(Pokemon wildPokemon) {
            Debug.Log("Encounter!");
            GameState = GameState.Battle;
            var wildEncounter = new WildPlayer(wildPokemon);
            var participants = new List<Player> { playerController.Player, wildEncounter };
        
            StartCoroutine(TransitionController.RunTransition(Transition.BattleEnter));
            yield return TransitionController.WaitForTransitionPeak;
            yield return battleWindow.OpenWindow(participants, true);

            var task = new Task(battleWindow.RunWindow());
            yield return new WaitWhile(() => task.Running);

            StartCoroutine(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return battleWindow.CloseWindow();
            yield return TransitionController.WaitForTransitionCompletion;
        
        
        }
    }
}