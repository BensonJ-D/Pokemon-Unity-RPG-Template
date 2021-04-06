
using System;
using System.Collections;
using Battle;
using Encounters;
using Misc;
using PokemonScripts;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private BattleController battleController;
    [SerializeField] private PlayerController player;
    [SerializeField] private TransitionController transitionController;
    
    public static GameState GameState { get; private set; } = GameState.Moving;
    public static TransitionState TransitionState { get; private set; } = TransitionState.None;

    private Task _playerMovement;

    private void Start()
    {
        EncounterRegion[] encounterRegions = FindObjectsOfType<EncounterRegion>();

        foreach (var region in encounterRegions)
        {
            region.OnEncountered += wildPokemon => StartCoroutine(StartBattle(player.Party, wildPokemon));
        }

        battleController.OnBattleOver += won => StartCoroutine(EndBattle(won));
    }
    
    private void Update()
    {
        switch (GameState)
        {
            case GameState.Moving:
            {
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

    private IEnumerator StartBattle(PokemonParty playerPokemon, Pokemon wildPokemon)
    {
        GameState = GameState.Battle;
     
        TransitionState = TransitionState.Start;
        yield return transitionController.StartTransition();
        
        battleController.gameObject.SetActive(true);
        StartCoroutine(battleController.SetupBattle(playerPokemon, wildPokemon));
        
        TransitionState = TransitionState.End;
        yield return transitionController.EndTransition();
        
        TransitionState = TransitionState.None;
    }

    private IEnumerator EndBattle(bool won)
    {
        TransitionState = TransitionState.Start;
        yield return transitionController.StartTransition();

        yield return battleController.Reset();
        
        TransitionState = TransitionState.End;
        yield return transitionController.EndTransition();
        
        GameState = GameState.Moving;
        TransitionState = TransitionState.None;
    }
}