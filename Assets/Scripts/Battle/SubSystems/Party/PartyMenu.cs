using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Menus;
using PokemonScripts;
using Scripts.Menus;
using UnityEngine;
using UnityEngine.UI;
using VFX;

namespace Battle.SubSystems.Party
{
    
    public class PartyMenu : SceneWindow
    {
        [SerializeField] private Text messageText;
        [SerializeField] private GameObject childWindow;
        [SerializeField] private List<PartySlot> partySlots;
        [SerializeField] private SummaryMenu summaryMenu;
        [SerializeField] private OptionWindow optionWindow;

        public enum PokemonChoice { Pokemon1 = 0, Pokemon2 = 1, Pokemon3 = 2, Pokemon4 = 3, Pokemon5 = 4, Pokemon6 = 5, Back = 6}
        public Dictionary<Participant, PokemonChoice> Choice { get; private set; }
        
        private List<int> _orderOfPokemon;
        private PokemonParty _party;

        private static class MenuOptions
        {
            public const string 
                Switch = "SWITCH",
                Summary = "SUMMARY",
                Cancel = "CANCEL";
        }
        
        public override void Init()
        {
            Scene = Scene.PartyView;
            Choice = new Dictionary<Participant, PokemonChoice> {
                {Participant.Player, PokemonChoice.Pokemon1}, 
                {Participant.Opponent, PokemonChoice.Pokemon1}
            };
            
            base.Init();
        }
        
        public void Reset()
        {
            Choice[Participant.Player] = PokemonChoice.Pokemon1;
            Choice[Participant.Opponent] = PokemonChoice.Pokemon1;
            State[Participant.Player] = SubsystemState.Closed;
            State[Participant.Opponent] = SubsystemState.Closed;
        }

        public void SetPartyData(PokemonParty newParty)
        {
            _party = newParty;
            _orderOfPokemon = _party.GetCurrentBattleOrder();
            
            var slotAndPokemon = _orderOfPokemon.Zip(partySlots, (p, s) => new {p, s});
            foreach (var pair in slotAndPokemon)
            {
                pair.s.SetData(_party.Party[pair.p]);
                pair.s.gameObject.SetActive(true);
            }
        }

        protected override void OnOpen(Participant participant) {
            _orderOfPokemon.ForEach(slot => partySlots[slot].SetSelected(false));
            partySlots[0].SetSelected(true);
            Choice[participant] = PokemonChoice.Pokemon1;
            messageText.text = "Choose a Pokemon.";
            
            base.OnOpen(participant);
        }
        
        // public IEnumerator OpenMenu(Participant participant, PokemonParty partyPokemon, Scene sourceScene)
        // {
        //     if (participant == Participant.Player)
        //     {
        //         yield return TransitionController.Instance.RunTransition(Transition.BattleEnter,
        //             OnTransitionPeak: () =>
        //             {
        //                 source = sourceScene;
        //                 SetPartyData(partyPokemon);
        //
        //                 orderOfPokemon.ForEach(slot => partySlots[slot].SetSelected(false));
        //                 partySlots[0].SetSelected(true);
        //                 Choice[participant] = PokemonChoice.Pokemon1;
        //                 State[participant] = SubsystemState.Open;
        //                 
        //                 SceneController.Instance.SetActiveScene(Scene.PartyView);
        //                 messageText.text = "";
        //             }
        //         );
        //     }
        // }

        protected override void OnClose(Participant participant)
        {
            messageText.text = "";
            base.OnClose(participant);
        }

        public IEnumerator HandlePokemonSelection(Participant participant, bool isCloseable = true)
        {
            if (participant == Participant.Player)
            {
                var oldSelection = (int) Choice[participant];
                var newSelection = Utils.GetPokemonOption((int) Choice[participant], _party.Party.Count);
                if (oldSelection != newSelection)
                {
                    partySlots[oldSelection].SetSelected(false);
                    partySlots[newSelection].SetSelected(true);
                    Choice[participant] = (PokemonChoice) newSelection;
                }
                
                if (Input.GetKeyDown(KeyCode.X) && isCloseable)
                {
                    Choice[participant] = PokemonChoice.Back;
                    yield return CloseWindow(participant);
                    yield break;
                }
                
                if (!Input.GetKeyDown(KeyCode.Z)) yield break;

                List<int> battleOrder = _party.GetCurrentBattleOrder();
                var indexForNewPokemon = battleOrder[newSelection];
                var selectedPokemon = _party.Party[indexForNewPokemon];
                optionWindow.SetOptions(new[,]
                {
                    {MenuOptions.Summary, MenuOptions.Switch},
                    {MenuOptions.Summary, null},
                    {MenuOptions.Cancel, null}
                });
                yield return optionWindow.ShowWindow();

                var action = optionWindow.Choice;
                switch (action)
                {
                    case MenuOptions.Summary:
                        summaryMenu.Init();
                        summaryMenu.SetPokemonData(selectedPokemon);
                        yield return summaryMenu.OpenMenu(participant, Scene.PartyView);
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.X));
                        yield return summaryMenu.CloseWindow(participant);
                        break;
                    case MenuOptions.Switch when selectedPokemon.CurrentHp <= 0:
                        SetMessageText("You can't send out a fainted pokemon!");
                        break;
                    case MenuOptions.Switch when newSelection == 0:
                        SetMessageText("You can't send out a Pokemon that's already in battle.");
                        break;
                    case MenuOptions.Switch:
                        _party.SetPokemonToBattleLeader(newSelection);
                        yield return CloseWindow(participant);
                        break;
                }
            }
            else
            {
                Choice[participant] = PokemonChoice.Back;
                yield return CloseWindow(participant);
            }
        }

        private void SetMessageText(string message)
        {
            messageText.text = message;
        }
    }
}