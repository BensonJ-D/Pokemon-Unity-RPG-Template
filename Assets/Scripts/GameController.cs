
using System;
using System.Collections;
using Battle;
using Encounters;
using Misc;
using PokemonScripts;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

public enum GameState { Start, Moving, Talking, Battle, Menu }

public class GameController : MonoBehaviour
{
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlayerController player;
    [SerializeField] private TransitionController transitionController;
    [SerializeField] private SceneController sceneController;
    
    public static GameState GameState { get; private set; } = GameState.Moving;

    private Task _playerMovement;
    private Task _popupMenu;
    private InputMap Keyboard;
    private Vector2Int arrowInput;
    
    public void Start()
    {
        Keyboard = new InputMap();
        Keyboard.Player.Enable();
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
                var isNotMoving = _playerMovement is null || !_playerMovement.Running || (_popupMenu != null && _popupMenu.Running);
                if (isNotMoving)
                {
                    if (_popupMenu != null && _popupMenu.Running) { break; }
                    
                    if(Keyboard.Player.Accept.triggered){
                        _popupMenu = new Task(OptionWindow.Instance.ShowWindow(new[,]
                        {
                            {"FUCK"},
                            {"YOU"},
                            {"INVENTORY"},
                            {"DIGIMON"},
                            {"SAVE"},
                            {"CUCK"}
                        }));
                    } else {
                        _playerMovement = new Task(player.HandleMovement());
                    }
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

    private IEnumerator StartBattle(PokemonParty playerPokemon, Inventory.Inventory playerInventory, Pokemon wildPokemon)
    {
        GameState = GameState.Battle;
        
        yield return transitionController.RunTransition(Transition.BattleEnter,
            OnTransitionPeak: () =>
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