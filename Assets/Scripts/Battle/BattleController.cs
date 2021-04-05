using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle.SubSystems;
using Misc;
using PokemonScripts;
using PokemonScripts.Moves;
using UnityEngine;
using VFX;
using Random = UnityEngine.Random;

namespace Battle
{
    public enum Participant { Player, Opponent }
    public enum BattleState { Start, Turn, Battle, End }
    public enum SubsystemState { Open, Closed }
    public enum TurnState { Busy, Ready }
    public enum BattleAction { NewPokemon = -3, Weather = -2, PersistentDamage = -1, Move = 0, Item = 1, Switch = 2, Run = 3 }

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
        private List<(BattleAction, IEnumerator, Participant)> _actions;

        private DamageDetails _attackResult;

        private void Start()
        {
            _pokemon = new Dictionary<Participant, BattlePokemon>
            {
                {Participant.Player, player.GetComponentInChildren<BattlePokemon>()},
                {Participant.Opponent, opponent.GetComponentInChildren<BattlePokemon>()}
            };
            _status = new Dictionary<Participant, BattleStatus>
            {
                {Participant.Player, player.GetComponentInChildren<BattleStatus>()},
                {Participant.Opponent, opponent.GetComponentInChildren<BattleStatus>()}
            };
            _turnState = new Dictionary<Participant, TurnState>
            {
                {Participant.Player, TurnState.Busy},
                {Participant.Opponent, TurnState.Busy}
            };
            _party = new Dictionary<Participant, PokemonParty>
            {
                {Participant.Player, null},
                {Participant.Opponent, null}
            };
            _actions = new List<(BattleAction, IEnumerator, Participant)>();
            
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
            var enterPokemon2 = new Task(_pokemon[Participant.Opponent].PlayEnterAnimation());

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

            while (actionMenu.State[participant] == SubsystemState.Open)
            {
                if (participant == Participant.Player)
                {
                    dialogBox.UpdateActionSelection((int) actionMenu.Choice[participant]);
                }

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

            if (participant == Participant.Player)
            {
                dialogBox.SetMoveNames(_pokemon[participant].Pokemon.Moves);
            }

            while (moveMenu.State[participant] == SubsystemState.Open)
            {
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
                    _actions.Add((BattleAction.Move, PerformMove(participant), participant));
                    _turnState[participant] = TurnState.Ready;
                    break;
            }
        }

        private IEnumerator ChoosePokemon(Participant participant)
        {
            partyMenu.OpenMenu(participant, _party[participant]);

            while (partyMenu.State[participant] == SubsystemState.Open)
            {
                yield return partyMenu.HandlePokemonSelection(participant);
            }

            switch (partyMenu.Choice[participant])
            {
                case PartyMenu.PokemonChoice.Back:
                    StartCoroutine(ChooseAction(participant));
                    break;
                default:
                    _actions.Add((BattleAction.Switch, PerformSwitch(participant), participant));
                    _turnState[participant] = TurnState.Ready;
                    break;
            }
        }

        private IEnumerator HandleBattle()
        {
            _actions.Sort(PrioritizeActions);

            while (_actions.Count > 0)
            {
                yield return _actions[0].Item2;
                _actions.RemoveAt(0);
            }
            
            if (BattleState == BattleState.Battle) BattleState = BattleState.Start;
            StartTurn();
        }

        private IEnumerator PerformMove(Participant participant)
        {
            if (BattleState != BattleState.Battle) yield break;
            

            var defendingParticipant = (Participant) (((int) participant + 1) % 2);
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

        private IEnumerator DealDamage(Participant attackingTrainer, BattleStatus display, Move move,
            BattlePokemon attacker, BattlePokemon defender)
        {
            _attackResult = CalculateDamage(move, attacker.Pokemon, defender.Pokemon);
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

            if (_attackResult.Critical)
            {
                yield return DisplayText("It's a critical hit!");
            }

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
                if (nextPokemon == null)
                {
                    BattleState = BattleState.End;
                    OnBattleOver?.Invoke(false);
                    yield break;
                }

                partyMenu.OpenMenu(Participant.Player, _party[Participant.Player]);
                while (partyMenu.State[Participant.Player] == SubsystemState.Open)
                {
                    yield return partyMenu.HandlePokemonSelection(Participant.Player, false);
                }

                yield return PerformSwitchIn(Participant.Player);
                BattleState = BattleState.Start;
            }

            foreach (var message in _attackResult.Messages)
            {
                yield return DisplayText(message);
            }
        }

        private IEnumerator PerformSwitch(Participant participant)
        {
            if (BattleState != BattleState.Battle) yield break;

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
            Task enterText =
                new Task(dialogBox.TypeDialog($"Let's go, {_pokemon[participant].Pokemon.Base.Species}!!"));
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

        private static DamageDetails CalculateDamage(Move move, Pokemon attacker, Pokemon defender)
        {
            var critical = (Random.value <= 0.0625f);
            var effectivenessMultiplier = MoveBase.TypeChart[(move.Base.Type, defender.Base.Type1)] *
                                          MoveBase.TypeChart[(move.Base.Type, defender.Base.Type2)];
            var typeAdvantage = MoveBase.GetEffectiveness(effectivenessMultiplier);

            var criticalMultiplier = critical ? 2.0f : 1.0f;
            var variability = Random.Range(0.85f, 1f);

            var atkVsDef = 0f;
            switch (move.Base.Category)
            {
                case MoveCategory.Physical:
                    atkVsDef = (float) attacker.BoostedAttack / attacker.BoostedDefence;
                    break;
                case MoveCategory.Special:
                    atkVsDef = (float) attacker.BoostedSpAttack / attacker.BoostedSpDefence;
                    break;
                case MoveCategory.Status:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var multiplier = effectivenessMultiplier * criticalMultiplier;
            var a = (2 * attacker.Level + 10) / 250f;
            var d = a * move.Base.Power * atkVsDef + 2;
            var damage = move.Base.Category != MoveCategory.Status 
                ? Mathf.FloorToInt(d * variability * multiplier)
                : 0;
            var fainted = defender.CurrentHp <= damage;

            List<string> messages = new List<string>();
            if (!fainted && move.Base.EffectChance >= Random.Range(1, 101))
            {
                messages = move.ApplyEffects(attacker, defender);
            }
            
            defender.CurrentHp = fainted ? 0 : defender.CurrentHp - damage;
            
            return new DamageDetails(critical, typeAdvantage, fainted, damage, multiplier, messages);
        }
        
        // public enum BattleAction { NewPokemon = -3, Weather = -2, PersistentDamage = -1, Move = 0, Item = 1, Switch = 2, Run = 3 }
        private int PrioritizeActions((BattleAction, IEnumerator, Participant) b, (BattleAction, IEnumerator, Participant) a)
        {
            var (action2, _, participant2) = b;
            var (action1, _, participant1) = a;
            if (action2 != action1) { return action1 - action2; }
            
            var coinFlip = Random.Range(0, 2) == 0 ? -1 : 1;
            
            if (action1 == BattleAction.Switch || action1 == BattleAction.Item) { return coinFlip; }

            var pokemon2 = _pokemon[participant2].Pokemon;
            var pokemon1 = _pokemon[participant1].Pokemon;
            var speed2 = pokemon2.BoostedSpeed;
            var speed1 = pokemon1.BoostedSpeed;
            var moveChoice2 = (int) moveMenu.Choice[participant2];
            var moveChoice1 = (int) moveMenu.Choice[participant1];
            var move2 = pokemon1.Moves[moveChoice2];
            var move1 = pokemon1.Moves[moveChoice1];
            
            if (action1 == BattleAction.Move)
            {
                if(move2.Base.Priority != move1.Base.Priority) { return move1.Base.Priority - move2.Base.Priority; }
            }

            if(speed2 != speed1) { return speed1 - speed2; }
            return coinFlip;
        }
    }
    
    public readonly struct DamageDetails
    {
        public readonly bool Critical;
        public readonly AttackEffectiveness Effective;
        public readonly bool Fainted;
        public readonly int DamageDealt;
        public readonly float Multiplier;
        public readonly List<string> Messages;

        public DamageDetails(bool critical, AttackEffectiveness effective, bool fainted, int damageDealt,
            float multiplier, List<string> messages)
        {
            Critical = critical;
            Effective = effective;
            Fainted = fainted;
            DamageDealt = damageDealt;
            Multiplier = multiplier;
            Messages = messages;
        }
    }
}