
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
    [SerializeField] private OptionWindow optionWindow;
    // [SerializeField] private GridMenu gridMenu;
    
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
                // if (Input.GetKeyDown(KeyCode.Q)) { _openMenu = new Task(OpenMenu()); }
                
                var isNotMoving = _playerMovement is null || !_playerMovement.Running;
                if (isNotMoving) { _playerMovement = new Task(player.HandleMovement()); }
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
        
        yield return transitionController.RunTransitionWithEffect(Transition.BattleEnter,
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
        yield return transitionController.RunTransitionWithEffect(Transition.BattleEnter,
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

    // private IEnumerator OpenMenu()
    // {
    //     GameState = GameState.Menu;
    //     gridMenu.SetOptions(new[,]
    //     {
    //         {"A", null, "C", null},
    //         {"A", "B", "C", "D"},
    //         {"A", null, "C", null},
    //     });
    //     yield return gridMenu.ShowWindow();
    //     GameState = GameState.Moving;
    // }
}