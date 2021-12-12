
using System;
using System.Collections;
using Battle;
using Encounters;
using Misc;
using Player;
using PokemonScripts;
using UnityEngine;
using VFX;

public enum GameState { Start, Moving, Talking, Battle, Menu }

public class GameController : MonoBehaviour
{
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlayerController player;
    [SerializeField] private TransitionController transitionController;
    [SerializeField] private SceneController sceneController;
    
    public static GameState GameState { get; private set; } = GameState.Moving;

    private Task playerMovement;

    private void Start()
    {
        EncounterRegion[] encounterRegions = FindObjectsOfType<EncounterRegion>();

        foreach (var region in encounterRegions)
        {
            region.OnEncountered += wildPokemon => StartCoroutine(StartBattle(player.Party, player.Inventory, wildPokemon));
        }

        battleController.OnBattleOver += won => StartCoroutine(EndBattle(won));
    }
    
    private void Update()
    {
        switch (GameState)
        {
            case GameState.Moving:
            {
                var isNotMoving = playerMovement is null || !playerMovement.Running;
                if (isNotMoving) { playerMovement = new Task(player.HandleMovement()); }
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

    private IEnumerator StartBattle(PokemonParty playerPokemon, Inventory.Inventory playerInventory, Pokemon wildPokemon)
    {
        GameState = GameState.Battle;
        
        yield return transitionController.RunTransition(Transition.BattleEnter,
            () =>
            {
                battleController.gameObject.SetActive(true);
                StartCoroutine(battleController.SetupBattle(playerPokemon, playerInventory, wildPokemon));
                sceneController.SetActiveScene(Scene.BattleView);
            }
        );
    }

    private IEnumerator EndBattle(bool won)
    {
        yield return transitionController.RunTransition(Transition.BattleEnter,
            () =>
            {
                StartCoroutine(battleController.Reset());
                sceneController.SetActiveScene(Scene.WorldView);
            },
            () =>
            {
                GameState = GameState.Moving;
            });
    }
}