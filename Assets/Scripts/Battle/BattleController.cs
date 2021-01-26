using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle.SubSystems;
using PokemonScripts;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

namespace Battle
{
    public enum Participant { Player, Opponent }
    public enum BattleState { Start, Turn, Battle, End }
    public enum SubsystemState { Open, Closed }
    public enum TurnState { Busy, Ready }

    public class BattleController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject opponent;
        [SerializeField] private ActionMenu actionMenu;
        [SerializeField] private MoveMenu moveMenu;
        [SerializeField] private PartyMenu partyMenu;
        [SerializeField] private BattleDialogBox dialogBox;
        
        public event Action<bool> OnBattleOver;
        
        private BattleState BattleState { get; set; } = BattleState.Start;
        private Dictionary<Participant, PokemonParty> _party;
        private Dictionary<Participant, TurnState> _turnState;
        private Dictionary<Participant, BattlePokemon> _pokemon;
        private Dictionary<Participant, BattleStatus> _status;
    
        private Pokemon.DamageDetails _attackResult;

        private void Start()
        {
            _pokemon = new Dictionary<Participant, BattlePokemon> {
                {Participant.Player, player.GetComponentInChildren<BattlePokemon>()}, 
                {Participant.Opponent, opponent.GetComponentInChildren<BattlePokemon>()}
            };
            _status = new Dictionary<Participant, BattleStatus> {
                {Participant.Player, player.GetComponentInChildren<BattleStatus>()}, 
                {Participant.Opponent, opponent.GetComponentInChildren<BattleStatus>()}
            };
            _turnState = new Dictionary<Participant, TurnState> {
                {Participant.Player, TurnState.Busy}, 
                {Participant.Opponent, TurnState.Busy}
            };
            _party = new Dictionary<Participant, PokemonParty> {
                {Participant.Player, null}, 
                {Participant.Opponent, null}
            };

            actionMenu.Init();
            moveMenu.Init();
            partyMenu.Init();
            
            gameObject.SetActive(false);
        }

        public IEnumerator SetupBattle(PokemonParty playerPokemon, Pokemon wildPokemon)
        {
            BattleState = BattleState.Start;

            actionMenu.Reset();
            moveMenu.Reset();
            partyMenu.Reset();
            
            _party[Participant.Player] = playerPokemon;
            _party[Participant.Player].ResetBattleOrder();
            
            _pokemon[Participant.Player].Setup(_party[Participant.Player].GetFirstBattleReadyPokemon());
            _pokemon[Participant.Opponent].Setup(wildPokemon);
        
            _status[Participant.Player].SetData(_pokemon[Participant.Player].Pokemon);
            _status[Participant.Opponent].SetData(_pokemon[Participant.Opponent].Pokemon);
            
            dialogBox.ClearText();
            
            var enterPokemon1 = new Task(_pokemon[Participant.Player].PlayEnterAnimation());
            var enterPokemon2 = new Task (_pokemon[Participant.Opponent].PlayEnterAnimation());

            yield return new WaitWhile(() => GameController.TransitionState != TransitionState.None 
                                             || enterPokemon1.Running || enterPokemon2.Running);
            
            yield return DisplayText($"A wild {_pokemon[Participant.Opponent].Pokemon.Base.Species} appeared!");
            yield return new WaitForSeconds(1f);

            BattleState = BattleState.Start;
            StartTurn();
        }

        private void StartTurn()
        {
            if (BattleState != BattleState.Start) return;
           
            _turnState[Participant.Player] = TurnState.Busy;
            _turnState[Participant.Opponent] = TurnState.Busy;

            BattleState = BattleState.Turn;
            StartCoroutine(ChooseAction(Participant.Player));
            StartCoroutine(ChooseAction(Participant.Opponent));
            StartCoroutine(WaitForParticipantsReady());
        }
        
        private IEnumerator ChooseAction(Participant participant)
        {
            actionMenu.OpenMenu(participant, $"What will {_pokemon[participant].Pokemon.Name} do?");
            yield return null;
            
            while (actionMenu.State[participant] == SubsystemState.Open) {
                if(participant == Participant.Player) { dialogBox.UpdateActionSelection((int) actionMenu.Choice[participant]); }
                yield return actionMenu.HandleActionSelection(participant);
            }

            switch (actionMenu.Choice[participant])
            {
                case ActionMenu.ActionChoice.Fight:
                    StartCoroutine(ChooseMove(participant));
                    break;
                case ActionMenu.ActionChoice.Bag:
                    break;
                case ActionMenu.ActionChoice.Pokemon:
                    StartCoroutine(ChoosePokemon(participant));
                    break;
                case ActionMenu.ActionChoice.Run:
                    BattleState = BattleState.End;
                    OnBattleOver?.Invoke(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        private IEnumerator ChooseMove(Participant participant)
        {
            moveMenu.OpenMenu(participant, _pokemon[participant].Pokemon.Moves);
            
            if (participant == Participant.Player) { dialogBox.SetMoveNames(_pokemon[participant].Pokemon.Moves); }
        
            while (moveMenu.State[participant] == SubsystemState.Open) {
                if (participant == Participant.Player)
                {
                    dialogBox.UpdateMoveSelection(moveMenu.Choice[participant],
                        _pokemon[participant].Pokemon.Moves[(int) moveMenu.Choice[participant]]);
                }

                yield return moveMenu.HandleMoveSelection(participant);
            }
            
            switch (moveMenu.Choice[participant])
            {
                case MoveMenu.MoveChoice.Back:
                    StartCoroutine(ChooseAction(participant));
                    break;
                default:
                    _turnState[participant] = TurnState.Ready;
                    break;
            }
        }

        private IEnumerator ChoosePokemon(Participant participant)
        {
            partyMenu.OpenMenu(participant, _party[participant]);
        
            while (partyMenu.State[participant] == SubsystemState.Open) {
                yield return partyMenu.HandlePokemonSelection(participant);
            }
            
            switch (partyMenu.Choice[participant])
            {
                case PartyMenu.PokemonChoice.Back:
                    StartCoroutine(ChooseAction(participant));
                    break;
                default:
                    _turnState[participant] = TurnState.Ready;
                    break;
            }
        }

        private IEnumerator HandleBattle()
        {

            if (actionMenu.Choice[Participant.Player] == ActionMenu.ActionChoice.Pokemon)
            {
                yield return PerformSwitch(Participant.Player);
                yield return PerformMove(Participant.Opponent);
            }
            else if (_pokemon[Participant.Player].Pokemon.Speed > _pokemon[Participant.Opponent].Pokemon.Speed)
            {
                yield return PerformMove(Participant.Player);
                yield return PerformMove(Participant.Opponent);
            }
            else
            {
                yield return PerformMove(Participant.Opponent);
                yield return PerformMove(Participant.Player);
            }
            
            if (BattleState == BattleState.Battle) BattleState = BattleState.Start;
            StartTurn();
        }

        private IEnumerator PerformMove(Participant participant)
        {
            if(BattleState != BattleState.Battle) yield break;
            
            var defendingParticipant = (Participant)(((int)participant + 1) % 2);
            var attacker = _pokemon[participant];
            var defender = _pokemon[defendingParticipant];
            var defendersHud = _status[defendingParticipant];
            var moveChoice = (int) moveMenu.Choice[participant];
            var move = attacker.Pokemon.Moves[moveChoice];
            yield return DisplayText($"{attacker.Pokemon.Base.Species} used {move.Base.Name}!");
            yield return attacker.PlayBasicHitAnimation();
            yield return defender.PlayDamageAnimation();
            yield return DealDamage(participant, defendersHud, move, attacker, defender);
        }

        private IEnumerator DealDamage(Participant attackingTrainer, 
            BattleStatus display, Move move, BattlePokemon attacker, BattlePokemon defender)
        {
            _attackResult = defender.Pokemon.TakeDamage(move, attacker.Pokemon);
            yield return display.UpdateHealthBar(_attackResult);
        
            switch (_attackResult.Effective)
            {
                case AttackEffectiveness.NoEffect:
                    yield return DisplayText("The move had no effect ...");
                    break;
                case AttackEffectiveness.NotVeryEffective:
                    yield return DisplayText("It's not very effective ...");
                    break;
                case AttackEffectiveness.SuperEffective:
                    yield return DisplayText("It's super effective!");
                    break;
                case AttackEffectiveness.NormallyEffective:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_attackResult.Critical) { yield return DisplayText("It's a critical hit!"); }
        
            if (_attackResult.Fainted)
            {
                yield return defender.PlayFaintAnimation();
                yield return DisplayText($"{defender.Pokemon.Base.Species} fainted!!");
                yield return new WaitForSeconds(1f);

                var won = attackingTrainer == Participant.Player;
                if (won)
                {
                    _party[Participant.Player].ResetBattleOrder();
                    BattleState = BattleState.End;
                    OnBattleOver?.Invoke(true);
                    yield break;
                }

                var nextPokemon = _party[Participant.Player].Party[1];
                if (nextPokemon == null){
                    BattleState = BattleState.End;
                    OnBattleOver?.Invoke(false);
                    yield break;
                }
                
                partyMenu.OpenMenu(Participant.Player, _party[Participant.Player]);
                while (partyMenu.State[Participant.Player] == SubsystemState.Open) {
                    yield return partyMenu.HandlePokemonSelection(Participant.Player, false);
                }

                yield return PerformSwitchIn(Participant.Player);
                BattleState = BattleState.Start;
            }
        }

        private IEnumerator PerformSwitch(Participant participant)
        {
            if(BattleState != BattleState.Battle) yield break;

            yield return DisplayText($"Good job, {_pokemon[participant].Pokemon.Base.Species}!");
            yield return _pokemon[participant].PlayFaintAnimation();
            yield return PerformSwitchIn(participant);
        }

        private IEnumerator PerformSwitchIn(Participant participant)
        {
            var newPokemon = _party[participant].GetFirstBattleReadyPokemon();
            _pokemon[participant].Setup(newPokemon);
            _status[participant].SetData(_pokemon[participant].Pokemon);
            Task enterAnimation = new Task(_pokemon[participant].PlayEnterAnimation());
            Task enterText = new Task(dialogBox.TypeDialog($"Let's go, {_pokemon[participant].Pokemon.Base.Species}!!"));
            yield return new WaitWhile(() => enterAnimation.Running || enterText.Running);
            yield return new WaitForSeconds(1f);
        }
        
        private IEnumerator WaitForParticipantsReady()
        {
            _turnState[Participant.Opponent] = TurnState.Ready;
            yield return null;
            yield return new WaitUntil(AreBothParticipantsReady);
            
            BattleState = BattleState.Battle;
            StartCoroutine(HandleBattle());
        
        }
        private bool AreBothParticipantsReady()
        {
            return (BattleState == BattleState.Turn &&
                    _turnState[Participant.Player] == TurnState.Ready &&
                    _turnState[Participant.Opponent] == TurnState.Ready);
        }
        
        private IEnumerator DisplayText(string text)
        {
            yield return dialogBox.TypeDialog(text);
        }
        
        private void SetText(string text)
        {
            dialogBox.SetText(text);
        }
        
        private void ClearDialogText()
        {
            dialogBox.ClearText();
        }

        public IEnumerator Reset()
        {
            yield return _pokemon[Participant.Opponent].ResetAnimation();
            yield return _pokemon[Participant.Player].ResetAnimation();
            gameObject.SetActive(false);
        }
    }
}