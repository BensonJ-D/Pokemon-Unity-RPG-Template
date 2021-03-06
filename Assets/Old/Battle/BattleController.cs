using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Transition;
using Battle.SubSystems;
using Battle.SubSystems.Party;
using Inventory;
using Misc;
using PokemonScripts;
using PokemonScripts.Conditions;
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
        [SerializeField] private SubSystems.ActionMenu actionMenu;
        [SerializeField] private MoveMenu moveMenu;
        [SerializeField] private PartyMenu partyMenu;
        // [SerializeField] private InventoryMenu inventoryMenu;
        [SerializeField] private BattleDialogBox dialogBox;

        public event Action<bool> OnBattleOver;

        private BattleState BattleState { get; set; } = BattleState.Start;
        private Dictionary<Participant, OldPokemonParty> _party;
        private Dictionary<Participant, Inventory.Inventory> _inventory;
        private Dictionary<Participant, TurnState> _turnState;
        private Dictionary<Participant, BattlePokemon> _pokemon;
        private List<(BattleAction, IEnumerator, Participant)> _actions;
        
        private void Start()
        {
            _pokemon = new Dictionary<Participant, BattlePokemon>
            {
                {Participant.Player, player.GetComponentInChildren<BattlePokemon>()},
                {Participant.Opponent, opponent.GetComponentInChildren<BattlePokemon>()}
            };
            _turnState = new Dictionary<Participant, TurnState>
            {
                {Participant.Player, TurnState.Busy},
                {Participant.Opponent, TurnState.Busy}
            };
            _party = new Dictionary<Participant, OldPokemonParty>
            {
                {Participant.Player, null},
                {Participant.Opponent, null}
            };
            _inventory = new Dictionary<Participant, Inventory.Inventory>
            {
                {Participant.Player, null},
                {Participant.Opponent, null}
            };
            
            _actions = new List<(BattleAction, IEnumerator, Participant)>();
            
            actionMenu.Init();
            moveMenu.Init();
            partyMenu.Init();
            // inventoryMenu.Init();

            gameObject.SetActive(false);
        }

        public IEnumerator SetupBattle(OldPokemonParty playerOldPokemon, Inventory.Inventory playerInventory, Pokemon wildPokemon)
        {
            BattleState = BattleState.Start;

            actionMenu.Reset();
            moveMenu.Reset();
            partyMenu.Reset();
            _actions.Clear();
            
            _party[Participant.Player] = playerOldPokemon;
            _party[Participant.Player].ResetBattleOrder();

            _inventory[Participant.Player] = playerInventory;

            _pokemon[Participant.Player].Setup(_party[Participant.Player].GetFirstBattleReadyPokemon());
            _pokemon[Participant.Opponent].Setup(wildPokemon);

            dialogBox.ClearText();

            var enterPokemon1 = new Task(_pokemon[Participant.Player].PlayEnterAnimation());
            var enterPokemon2 = new Task(_pokemon[Participant.Opponent].PlayEnterAnimation());

            yield return new WaitWhile(() => TransitionController.TransitionState != TransitionState.None
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
                case SubSystems.ActionMenu.ActionChoice.Fight:
                    StartCoroutine(ChooseMove(participant));
                    break;
                case SubSystems.ActionMenu.ActionChoice.Bag:
                    StartCoroutine(ChooseItem(participant));
                    break;
                case SubSystems.ActionMenu.ActionChoice.Pokemon:
                    StartCoroutine(ChoosePokemon(participant));
                    break;
                case SubSystems.ActionMenu.ActionChoice.Run:
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

            if (moveMenu.Choice[participant] == MoveMenu.MoveChoice.Back)
            {
                StartCoroutine(ChooseAction(participant));
            }
            else
            {
                _actions.Add((BattleAction.Move, PerformMove(participant), participant));
                _turnState[participant] = TurnState.Ready;
            }
        }

        private IEnumerator ChoosePokemon(Participant participant)
        {
            partyMenu.SetPartyData(_party[participant]);
            yield return partyMenu.OpenMenu(participant, Scene.BattleView);

            while (partyMenu.State[participant] == SubsystemState.Open)
            {
                yield return partyMenu.HandlePokemonSelection(participant);
            }

            if (partyMenu.Choice[participant] == PartyMenu.PokemonChoice.Back)
            {
                StartCoroutine(ChooseAction(participant));
            }
            else
            {
                _actions.Add((BattleAction.Switch, PerformSwitch(participant), participant));
                _turnState[participant] = TurnState.Ready;
            }
        }

        private IEnumerator ChooseItem(Participant participant)
        {
            // inventoryMenu.SetInventoryData(_inventory[participant]);
            // yield return inventoryMenu.OpenMenu(participant, Scene.BattleView);
            //
            // while (inventoryMenu.State[participant] == SubsystemState.Open)
            // {
            //     yield return inventoryMenu.HandleItemSelection(participant);
            // }
            yield return null;
            StartCoroutine(ChooseAction(participant));
        }
        
        private IEnumerator HandleBattle()
        {
            _actions.Add((BattleAction.PersistentDamage, HandleStatusConditionsAfterTurn(Participant.Opponent), Participant.Opponent));
            _actions.Add((BattleAction.PersistentDamage, HandleStatusConditionsAfterTurn(Participant.Player), Participant.Player));
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
            var moveChoice = (int) moveMenu.Choice[participant];
            var move = attacker.Pokemon.Moves[moveChoice];
            var damageDetails = CalculateDamage(move, attacker.Pokemon, defender.Pokemon);
            yield return DisplayText($"{attacker.Pokemon.Base.Species} used {move.Base.Name}!");
            yield return attacker.PlayBasicHitAnimation();
            yield return defender.PlayDamageAnimation();
            yield return ApplyDamage(defender, damageDetails);
            yield return CheckForFaintedPokemon(attacker, defender, damageDetails);
            yield return ApplyEffects(move, attacker, defender, damageDetails);
        }

        private IEnumerator ApplyEffects(Move move, BattlePokemon attacker, 
            BattlePokemon defender, DamageDetails damageDetails)
        {
            if (move.Base.EffectChance < Random.Range(1, 101)) yield break;
            
            IEnumerable<string> result = move.ApplyEffects(attacker, defender);
            foreach (var s in result)
            {
                yield return DisplayText(s);
            }
        }
        
        private IEnumerator ApplyDamage(BattlePokemon defender, DamageDetails attackResult)
        {
            yield return defender.UpdateHealth(attackResult);

            switch (attackResult.Effective)
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

            if (attackResult.Critical)
            {
                yield return DisplayText("It's a critical hit!");
            }
        }

        private IEnumerator CheckForFaintedPokemon(BattlePokemon attacker, BattlePokemon defender, DamageDetails attackResult)
        {
            if (!attackResult.Fainted) yield break;
            
            yield return defender.PlayFaintAnimation();
            yield return DisplayText($"{defender.Pokemon.Base.Species} fainted!!");
            yield return new WaitForSeconds(1f);

            yield return DisplayText($"{attacker.Pokemon.Name} gained {defender.Pokemon.ExperienceYield} exp.");
            yield return attacker.UpdateExperience(defender.Pokemon.ExperienceYield);
            yield return new WaitForSeconds(1f);
            
            var won = defender == _pokemon[Participant.Opponent];
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

            partyMenu.SetPartyData(_party[Participant.Player]);
            yield return partyMenu.OpenMenu(Participant.Player, Scene.BattleView);
            while (partyMenu.State[Participant.Player] == SubsystemState.Open)
            {
                yield return partyMenu.HandlePokemonSelection(Participant.Player, false);
            }

            yield return PerformSwitchIn(Participant.Player);
            BattleState = BattleState.Start;
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
            Task enterAnimation = new Task(_pokemon[participant].PlayEnterAnimation());
            Task enterText =
            new Task(dialogBox.TypeDialog($"Let's go, {_pokemon[participant].Pokemon.Base.Species}!!"));
            yield return new WaitWhile(() => enterAnimation.Running || enterText.Running);
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator HandleStatusConditionsAfterTurn(Participant participant)
        {
            var primaryCondition = _pokemon[participant].Pokemon.PrimaryCondition;
            yield return null;
            yield return PrimaryStatusConditions.GetEffectClass[primaryCondition]
                .OnAfterTurn(_pokemon[participant], dialogBox);
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
            return BattleState == BattleState.Turn &&
                   _turnState[Participant.Player] == TurnState.Ready &&
                   _turnState[Participant.Opponent] == TurnState.Ready;
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

        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        public IEnumerator Reset()
        {
            yield return _pokemon[Participant.Opponent].ResetAnimation();
            yield return _pokemon[Participant.Player].ResetAnimation();
            gameObject.SetActive(false);
            
            yield return null;
        }

        private static int ApplyCriticalBonus(int damage)
        {
            return damage * 3 / 2;
        }
        
        private static int ApplySameTypeAttackBonus(int damage)
        {
            return damage * 3 / 2;
        }
        
        private static DamageDetails CalculateDamage(Move move, Pokemon attacker, Pokemon defender)
        {
            var critical = Random.value <= 0.0625f;
            var effectivenessMultiplier = MoveBase.TypeChart[(move.Base.Type, defender.Base.Type1)] *
                                          MoveBase.TypeChart[(move.Base.Type, defender.Base.Type2)];
            var typeAdvantage = MoveBase.GetEffectiveness(effectivenessMultiplier);

            var criticalMultiplier = critical ? 2.0f : 1.0f;
            var variability = Random.Range(0.85f, 1f);

            var attack = 0;
            var defence = 0;
            switch (move.Base.Category)
            {
                case MoveCategory.Physical:
                    attack = attacker.BoostedAttack;
                    defence = defender.BoostedDefence;
                    break;
                case MoveCategory.Special:
                    attack = attacker.BoostedSpAttack;
                    defence = defender.BoostedSpDefence;
                    break;
                case MoveCategory.Status:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var multiplier = effectivenessMultiplier * criticalMultiplier;
            var a = 2 * attacker.Level / 5;
            var b = a * move.Base.Power * attack / defence;
            var c = b / 50 + 2;
            var damage = move.Base.Category != MoveCategory.Status 
                ? Mathf.FloorToInt(c * variability * multiplier)
                : 0;
            var fainted = defender.CurrentHp <= damage;
            return new DamageDetails(critical, typeAdvantage, fainted, damage, multiplier);
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
            var move2 = pokemon2.Moves[moveChoice2];
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

        public DamageDetails(bool critical, AttackEffectiveness effective, bool fainted, int damageDealt,
            float multiplier)
        {
            Critical = critical;
            Effective = effective;
            Fainted = fainted;
            DamageDealt = damageDealt;
            Multiplier = multiplier;
        }
        
        public DamageDetails(bool fainted, int damageDealt)
        {
            Critical = false;
            Effective = AttackEffectiveness.NormallyEffective;
            Fainted = fainted;
            DamageDealt  = damageDealt;
            Multiplier = 1f;
        }
    }
}