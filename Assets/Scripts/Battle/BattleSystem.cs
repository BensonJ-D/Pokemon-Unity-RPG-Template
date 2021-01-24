using System;
using System.Collections;
using System.Collections.Generic;
using Pokemon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Battle
{
    public enum Participant { Player, Opponent }
    public enum BattleState { Start, Turn, Battle, End }
    public enum TurnState { ChooseAction, ChooseMove, Ready }
    public enum Action { Fight , Run }
    public enum MoveChoice { Move1 = 0, Move2 = 1, Move3 = 2, Move4 = 3 }

    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject opponent;

        [SerializeField] private BattleDialogBox dialogBox;

        private BattleState _battleState = BattleState.Start;
        private Dictionary<Participant, TurnState> _turnState;
        private Dictionary<Participant, Action> _action;
        private Dictionary<Participant, MoveChoice> _moveChoice;
        private Dictionary<Participant, BattlePokemon> _pokemon;
        private Dictionary<Participant, BattleStatus> _status;
    
        private Coroutine _parallelTextDisplay;
        private Pokemon.Pokemon.DamageDetails _attackResult;

        public void Start()
        {
            _pokemon = new Dictionary<Participant, BattlePokemon> {
                {Participant.Player, player.GetComponentInChildren<BattlePokemon>()}, 
                {Participant.Opponent, opponent.GetComponentInChildren<BattlePokemon>()}
            };
            _status = new Dictionary<Participant, BattleStatus> {
                {Participant.Player, player.GetComponentInChildren<BattleStatus>()}, 
                {Participant.Opponent, opponent.GetComponentInChildren<BattleStatus>()}
            };
            
            gameObject.SetActive(false);
        }
        public IEnumerator SetupBattle(Pokemon.Pokemon wildPokemon)
        {
            _battleState = BattleState.Start;
        
            _action = new Dictionary<Participant, Action> {
                {Participant.Player, Action.Fight}, 
                {Participant.Opponent, Action.Fight}
            };
            _turnState = new Dictionary<Participant, TurnState> {
                {Participant.Player, TurnState.ChooseAction}, 
                {Participant.Opponent, TurnState.ChooseAction}
            };
            _moveChoice = new Dictionary<Participant, MoveChoice> {
                {Participant.Player, MoveChoice.Move1}, 
                {Participant.Opponent, MoveChoice.Move1}
            };

            _pokemon[Participant.Player].Setup();
            _pokemon[Participant.Opponent].Setup(wildPokemon);
        
            _status[Participant.Player].SetData(_pokemon[Participant.Player].Pokemon);
            _status[Participant.Opponent].SetData(_pokemon[Participant.Opponent].Pokemon);
        
            yield return DisplayText($"A wild {_pokemon[Participant.Opponent].Pokemon.Base.Species} appeared!");
            yield return new WaitForSeconds(1f);

            _battleState = BattleState.Turn;
            StartCoroutine(ChooseAction(Participant.Player));
            StartCoroutine(ChooseAction(Participant.Opponent));
        }

        private void StartTurn()
        {
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableMoveSelector(false);
        
            _action[Participant.Player] = Action.Fight;
            _action[Participant.Opponent] = Action.Fight;

            _turnState[Participant.Player] = TurnState.ChooseAction;
            _turnState[Participant.Opponent] = TurnState.ChooseAction;

            _battleState = BattleState.Turn;
            StartCoroutine(ChooseAction(Participant.Player));
            StartCoroutine(ChooseAction(Participant.Opponent));
        }
        private IEnumerator ChooseAction(Participant participant)
        {
            _turnState[participant] = TurnState.ChooseAction;
            if(participant == Participant.Player) {
                ClearDialogText();
                dialogBox.EnableActionSelector(true);
                dialogBox.EnableDialogText(true);
                dialogBox.EnableMoveSelector(false);
                _parallelTextDisplay = StartCoroutine(DisplayText("Choose an action."));
            }

            while (_turnState[participant] == TurnState.ChooseAction) {
                yield return HandleActionSelection(participant);
            }
            yield return null;
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
            yield return null;
        }

        private IEnumerator HandleBattle()
        {
            _battleState = BattleState.Battle;
        
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(true);
            dialogBox.EnableMoveSelector(false);

            if (_pokemon[Participant.Player].Pokemon.Speed > _pokemon[Participant.Opponent].Pokemon.Speed)
            {
                yield return PerformMove(Participant.Player);
                yield return PerformMove(Participant.Opponent);
            }
            else
            {
                yield return PerformMove(Participant.Opponent);
                yield return PerformMove(Participant.Player);
            }

            StartTurn();
        }

        private IEnumerator PerformMove(Participant participant)
        {
            var defendingParticipant = (Participant)(((int)participant + 1) % 2);
            var attacker = _pokemon[participant];
            var defender = _pokemon[defendingParticipant];
            var defendersHud = _status[defendingParticipant];
            var moveChoice = (int)_moveChoice[participant];
            var move = attacker.Pokemon.Moves[moveChoice];
            yield return DisplayText($"{attacker.Pokemon.Base.Species} used {move.Base.Name}!");
            yield return DealDamage(defendersHud, move, attacker.Pokemon, defender.Pokemon);
        }

        private IEnumerator DealDamage(BattleStatus display, Move move, Pokemon.Pokemon attacker, Pokemon.Pokemon defender)
        {
            _attackResult = defender.TakeDamage(move, attacker);
            yield return display.UpdateHealthBar(_attackResult.DamageDealt);
        
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
                if(_parallelTextDisplay != null) {StopCoroutine(_parallelTextDisplay);}
                yield return DisplayText($"{defender.Base.Species} fainted!!");
                // TODO: EndBattle();
            }
            else
            {
                // NextTurn();
            }
        }

        private IEnumerator HandleActionSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow)) { _action[participant] = Action.Run; }
                else if (Input.GetKeyDown(KeyCode.UpArrow)) { _action[participant] = Action.Fight; }
                dialogBox.UpdateActionSelection(_action[participant]);

                if (!Input.GetKeyDown(KeyCode.Z)) yield return null;
                else if (_action[participant] == Action.Fight)
                {
                    yield return null;
                    StartCoroutine(ChooseMove(participant));
                }
                else if (_action[participant] == Action.Run)
                {
                    yield return null;
                    gameObject.SetActive(false);
                    Game.State = GameState.Moving;
                }
            }
            else
            {
                StartCoroutine(ChooseMove(participant));
            }
        }
    
        private IEnumerator HandleMoveSelection(Participant participant)
        {
            if (participant == Participant.Player)
            {
                var moves = _pokemon[participant].Pokemon.Moves;
                var nextMove = (int) _moveChoice[participant];

                if (Input.GetKeyDown(KeyCode.DownArrow) && nextMove < moves.Count - 2)
                {
                    nextMove = (int) _moveChoice[participant] + 2;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) && nextMove > 1)
                {
                    nextMove = (int) _moveChoice[participant] - 2;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) && nextMove % 2 == 0)
                {
                    nextMove = (int) _moveChoice[participant] + 1;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) && nextMove % 2 == 1)
                {
                    nextMove = (int) _moveChoice[participant] - 1;
                }

                _moveChoice[participant] = (MoveChoice) nextMove;

                dialogBox.UpdateMoveSelection(_moveChoice[participant],
                    _pokemon[participant].Pokemon.Moves[(int) _moveChoice[participant]]);
            
                if (!Input.GetKeyDown(KeyCode.Z)) yield return null;
                else
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

        public void Update()
        {
            if (BothParticipantsReady())
            {
                StartCoroutine(HandleBattle());
            }
        }

        private bool BothParticipantsReady()
        {
            return (_battleState == BattleState.Turn &&
                    _turnState[Participant.Player] == TurnState.Ready &&
                    _turnState[Participant.Opponent] == TurnState.Ready);
        }
        private IEnumerator DisplayText(string text)
        {
            yield return dialogBox.TypeDialog(text);
        }

        private void ClearDialogText()
        {
            if (_parallelTextDisplay != null)
            {
                StopCoroutine(_parallelTextDisplay);
            }
        }
    }
}