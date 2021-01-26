using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PokemonScripts;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

namespace Battle
{
    public enum Participant { Player, Opponent }
    public enum BattleState { None, Start, Turn, Menu, Battle, End, Destroy }
    public enum TurnState { ChooseAction, ChooseMove, ChoosePokemon, Ready }
    public enum ActionChoice { Fight = 0, Bag = 1, Pokemon = 2, Run = 3 }
    public enum MoveChoice { Move1 = 0, Move2 = 1, Move3 = 2, Move4 = 3 }

    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject opponent;
        [SerializeField] private BattleDialogBox dialogBox;
        [SerializeField] private PartyMenu partyMenu;

        public event Action<bool> OnBattleOver;
        private BattleState BattleState { get; set; } = BattleState.None;
        private Dictionary<Participant, PokemonParty> _party;
        private Dictionary<Participant, int> _pokemonChoice;
        private Dictionary<Participant, TurnState> _turnState;
        private Dictionary<Participant, ActionChoice> _actionChoice;
        private Dictionary<Participant, MoveChoice> _moveChoice;
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
            _actionChoice = new Dictionary<Participant, ActionChoice> {
                {Participant.Player, ActionChoice.Fight}, 
                {Participant.Opponent, ActionChoice.Fight}
            };
            _turnState = new Dictionary<Participant, TurnState> {
                {Participant.Player, TurnState.ChooseAction}, 
                {Participant.Opponent, TurnState.ChooseAction}
            };
            _moveChoice = new Dictionary<Participant, MoveChoice> {
                {Participant.Player, MoveChoice.Move1}, 
                {Participant.Opponent, MoveChoice.Move1}
            };
            _party = new Dictionary<Participant, PokemonParty> {
                {Participant.Player, null}, 
                {Participant.Opponent, null}
            };
            _pokemonChoice = new Dictionary<Participant, int> {
                {Participant.Player, 0}, 
                {Participant.Opponent, 0}
            };
            
            gameObject.SetActive(false);
        }

        public IEnumerator SetupBattle(PokemonParty playerPokemon, Pokemon wildPokemon)
        {
            BattleState = BattleState.Start;

            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelector(false);

            partyMenu.Init();

            _party[Participant.Player] = playerPokemon;
            _party[Participant.Player].ResetBattleOrder();
            
            _pokemon[Participant.Player].Setup(_party[Participant.Player].GetFirstBattleReadyPokemon());
            _pokemon[Participant.Opponent].Setup(wildPokemon);
        
            _status[Participant.Player].SetData(_pokemon[Participant.Player].Pokemon);
            _status[Participant.Opponent].SetData(_pokemon[Participant.Opponent].Pokemon);
            
            _moveChoice[Participant.Player] = MoveChoice.Move1; 
            _moveChoice[Participant.Opponent] = MoveChoice.Move1; 

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
            
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableMoveSelector(false);
        
            _actionChoice[Participant.Player] = ActionChoice.Fight;
            _actionChoice[Participant.Opponent] = ActionChoice.Fight;

            _turnState[Participant.Player] = TurnState.ChooseAction;
            _turnState[Participant.Opponent] = TurnState.ChooseAction;

            BattleState = BattleState.Turn;
            StartCoroutine(ChooseAction(Participant.Player));
            StartCoroutine(ChooseAction(Participant.Opponent));
            StartCoroutine(WaitForParticipantsReady());
        }
        
        private IEnumerator ChooseAction(Participant participant)
        {
            _turnState[participant] = TurnState.ChooseAction;
            if(participant == Participant.Player) {
                ClearDialogText();
                dialogBox.EnableActionSelector(true);
                dialogBox.EnableDialogText(true);
                dialogBox.EnableMoveSelector(false);
                SetText($"What will {_pokemon[participant].Pokemon.Base.Species} do?");
            }

            while (_turnState[participant] == TurnState.ChooseAction) {
                yield return HandleActionSelection(participant);
            }
        }
    
        private IEnumerator HandleActionSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                _actionChoice[participant] = (ActionChoice)Utils.GetGridOption((int) _actionChoice[participant], 2, 4);
                dialogBox.UpdateActionSelection(_actionChoice[participant]);

                if (!Input.GetKeyDown(KeyCode.Z)) yield break;
                
                switch (_actionChoice[participant])
                {
                    case ActionChoice.Fight:
                        yield return null;
                        StartCoroutine(ChooseMove(participant));
                        break;
                    
                    case ActionChoice.Pokemon:
                        yield return null;
                        StartCoroutine(ChoosePokemon(participant));
                        break;
                    case ActionChoice.Run:
                        yield return null;
                        BattleState = BattleState.Destroy;
                        OnBattleOver?.Invoke(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                StartCoroutine(ChooseMove(participant));
            }
        }
        
        private IEnumerator ChooseMove(Participant participant)
        {
            _turnState[participant] = TurnState.ChooseMove;
            if (participant == Participant.Player)
            {
                ClearDialogText();
                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(false);
                dialogBox.EnableMoveSelector(true);
                dialogBox.SetMoveNames(_pokemon[participant].Pokemon.Moves);
            }
        
            while (_turnState[participant] == TurnState.ChooseMove) {
                yield return HandleMoveSelection(participant);
            }
        }

        private IEnumerator HandleMoveSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                List<Move> moves = _pokemon[participant].Pokemon.Moves;
                _moveChoice[participant] = (MoveChoice)Utils.GetGridOption((int) _moveChoice[participant], 2, moves.Count);
                
                dialogBox.UpdateMoveSelection(_moveChoice[participant],
                    _pokemon[participant].Pokemon.Moves[(int) _moveChoice[participant]]);
            
                
                if (Input.GetKeyDown(KeyCode.X))
                {
                    _turnState[participant] = TurnState.ChooseAction;
                    StartCoroutine(ChooseAction(participant));
                    yield break;
                }
                
                if (!Input.GetKeyDown(KeyCode.Z)) yield return null;
                else if((int) _moveChoice[participant] < moves.Count)
                {
                    yield return null;
                    _turnState[participant] = TurnState.Ready;
                }
            }
            else
            {
                _moveChoice[participant] = (MoveChoice) Random.Range(0, _pokemon[participant].Pokemon.Moves.Count);
                _turnState[participant] = TurnState.Ready;
            }
        }
        
        private IEnumerator ChoosePokemon(Participant participant)
        {
            _turnState[participant] = TurnState.ChoosePokemon;
            if (participant == Participant.Player)
            {
                ClearDialogText();
                dialogBox.EnableActionSelector(false);
                dialogBox.EnableDialogText(false);
                dialogBox.EnableMoveSelector(false);
                
                BattleState = BattleState.Menu;
                _pokemonChoice[participant] = 0;
                partyMenu.SetPartyData(_party[participant]);
                partyMenu.EnablePartyMenu(true);
            }
        
            while (_turnState[participant] == TurnState.ChoosePokemon) {
                yield return HandlePokemonSelection(participant);
            }
        }
        
        private IEnumerator HandlePokemonSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                PokemonParty party = _party[participant];
                var nextMon = Utils.GetPokemonOption(_pokemonChoice[participant], party.Party.Count);

                if (nextMon != _pokemonChoice[participant])
                {
                    partyMenu.SetSelected(nextMon);
                    _pokemonChoice[participant] = nextMon;
                }
            
                
                if (Input.GetKeyDown(KeyCode.X))
                {
                    BattleState = BattleState.Turn;
                    _turnState[participant] = TurnState.ChooseAction;
                    partyMenu.EnablePartyMenu(false);
                    StartCoroutine(ChooseAction(participant));
                    yield break;
                }
                
                if (!Input.GetKeyDown(KeyCode.Z)) yield break;

                List<int> battleOrder = _party[participant].GetCurrentBattleOrder();
                var indexForNewPokemon = battleOrder[nextMon];
                var selectedPokemon = _party[participant].Party[indexForNewPokemon];

                if (selectedPokemon.Hp <= 0)
                {
                    partyMenu.SetMessageText("You can't send out a fainted pokemon!");
                }
                else if (_pokemonChoice[participant] == 0)
                {
                    partyMenu.SetMessageText("You can't send out a Pokemon that's already in battle.");
                }
                else
                {
                    _party[participant].SetPokemonToBattleLeader(nextMon);
                    partyMenu.EnablePartyMenu(false);
                    BattleState = BattleState.Turn;
                    _turnState[participant] = TurnState.Ready;
                }
            }
            else
            {
                _moveChoice[participant] = (MoveChoice) Random.Range(0, _pokemon[participant].Pokemon.Moves.Count);
                _turnState[participant] = TurnState.Ready;
            }
        }
        
        private IEnumerator HandleBattle()
        {
            if(BattleState != BattleState.Battle) yield break;
        
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelector(false);

            if (_actionChoice[Participant.Player] == ActionChoice.Pokemon)
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
            var moveChoice = (int)_moveChoice[participant];
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
                    BattleState = BattleState.Destroy;
                    OnBattleOver?.Invoke(true);
                    yield break;
                }

                var nextPokemon = _party[Participant.Player].Party[1];
                if (nextPokemon == null){
                    BattleState = BattleState.Destroy;
                    OnBattleOver?.Invoke(false);
                    yield break;
                }
                
                _pokemon[Participant.Player].Setup(nextPokemon);
                _status[Participant.Player].SetData(_pokemon[Participant.Player].Pokemon);
                _moveChoice[Participant.Player] = MoveChoice.Move1; 

                dialogBox.ClearText();
                
                var enterPokemon = new Task(_pokemon[Participant.Player].PlayEnterAnimation());
                var enterPokemonDialogue = new Task (DisplayText($"Go {nextPokemon.Base.Species}!!"));
                
                yield return new WaitWhile(() => enterPokemon.Running || enterPokemonDialogue.Running);
                yield return new WaitForSeconds(1f);
                BattleState = BattleState.Start;
            }
        }

        private IEnumerator PerformSwitch(Participant participant)
        {
            if(BattleState != BattleState.Battle) yield break;

            yield return DisplayText($"Good job, {_pokemon[participant].Pokemon.Base.Species}!");
            yield return _pokemon[participant].PlayFaintAnimation();
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