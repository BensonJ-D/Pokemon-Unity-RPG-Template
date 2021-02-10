using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle.SubSystems;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class PartyMenu : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private GameObject childWindow;
        [SerializeField] private List<PartySlot> partySlots;
        
        public enum PokemonChoice { Pokemon1 = 0, Pokemon2 = 1, Pokemon3 = 2, Pokemon4 = 3, Pokemon5 = 4, Pokemon6 = 5, Back = 6}
        
        public Dictionary<Participant, PokemonChoice> Choice { get; private set; }
        public Dictionary<Participant, SubsystemState> State { get; private set; }
        
        private List<int> _orderOfPokemon;
        private PokemonParty _party;

        public void Init()
        {
            Choice = new Dictionary<Participant, PokemonChoice> {
                {Participant.Player, PokemonChoice.Pokemon1}, 
                {Participant.Opponent, PokemonChoice.Pokemon1}
            };
            
            State = new Dictionary<Participant, SubsystemState> {
                {Participant.Player, SubsystemState.Closed}, 
                {Participant.Opponent, SubsystemState.Closed}
            };
        }
        
        public void Reset()
        {
            Choice[Participant.Player] = PokemonChoice.Pokemon1;
            Choice[Participant.Opponent] = PokemonChoice.Pokemon1;
            State[Participant.Player] = SubsystemState.Closed;
            State[Participant.Opponent] = SubsystemState.Closed;
        }

        private void SetPartyData(PokemonParty newParty)
        {
            _party = newParty;
            _orderOfPokemon = _party.GetCurrentBattleOrder();
            
            var slotAndPokemon = _orderOfPokemon.Zip(partySlots, (p, s) => new {p, s});
            foreach (var pair in slotAndPokemon)
            {
                pair.s.SetData(_party.Party[pair.p]);
                pair.s.gameObject.SetActive(true);
            }

            messageText.text = "Choose a Pokemon.";
        }

        public void OpenMenu(Participant participant, PokemonParty partyPokemon)
        {
            if (participant == Participant.Player)
            {
                childWindow.SetActive(true);
                messageText.text = "";
            }

            SetPartyData(partyPokemon);

            _orderOfPokemon.ForEach(slot => partySlots[slot].SetSelected(false));
            partySlots[0].SetSelected(true);
            Choice[participant] = PokemonChoice.Pokemon1;
            State[participant] = SubsystemState.Open;
        }

        private void CloseWindow(Participant participant)
        {
            if (participant == Participant.Player)
            {
                childWindow.SetActive(false);
            }
            State[participant] = SubsystemState.Closed;
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
                    CloseWindow(participant);
                    yield break;
                }
                
                if (!Input.GetKeyDown(KeyCode.Z)) yield break;
                
                List<int> battleOrder = _party.GetCurrentBattleOrder();
                var indexForNewPokemon = battleOrder[newSelection];
                var selectedPokemon = _party.Party[indexForNewPokemon];
                
                if (selectedPokemon.CurrentHp <= 0)
                {
                    SetMessageText("You can't send out a fainted pokemon!");
                }
                else if (newSelection == 0)
                {
                    SetMessageText("You can't send out a Pokemon that's already in battle.");
                }
                else
                {
                    _party.SetPokemonToBattleLeader(newSelection);
                    CloseWindow(participant);
                }
            }
            else
            {
                Choice[participant] = PokemonChoice.Back;
                CloseWindow(participant);
            }
        }

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }
    }
}