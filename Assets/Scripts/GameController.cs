
using System;
using System.Collections;
using Battle;
using Encounters;
using PokemonScripts;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

public enum GameState { Start, Moving, Talking, Battle, Menu }
public enum TransitionState { None, Start, End }

public class GameController : MonoBehaviour
{
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private PlayerController player;
    [SerializeField] private Transition transition;
    
    public static GameState GameState { get; private set; } = GameState.Moving;
    public static TransitionState TransitionState { get; private set; } = TransitionState.None;

    private Task playerMovement;

    private void Start()
    {
        EncounterRegion[] encounterRegions = FindObjectsOfType<EncounterRegion>();

        foreach (var region in encounterRegions)
        {
            region.OnEncountered += wildPokemon => StartCoroutine(StartBattle(player.Party, wildPokemon));
        }

        battleSystem.OnBattleOver += won => StartCoroutine(EndBattle(won));
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

    private IEnumerator StartBattle(PokemonParty playerPokemon, Pokemon wildPokemon)
    {
        GameState = GameState.Battle;
     
        TransitionState = TransitionState.Start;
        yield return transition.StartTransition();
        
        battleSystem.gameObject.SetActive(true);
        StartCoroutine(battleSystem.SetupBattle(playerPokemon, wildPokemon));
        
        TransitionState = TransitionState.End;
        yield return transition.EndTransition();
        
        TransitionState = TransitionState.None;
    }

    private IEnumerator EndBattle(bool won)
    {
        TransitionState = TransitionState.Start;
        yield return transition.StartTransition();

        yield return battleSystem.Reset();
        
        TransitionState = TransitionState.End;
        yield return transition.EndTransition();
        
        GameState = GameState.Moving;
        TransitionState = TransitionState.None;
    }
}