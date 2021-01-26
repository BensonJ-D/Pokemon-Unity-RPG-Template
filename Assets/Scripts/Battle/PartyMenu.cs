using System.Collections.Generic;
using System.Linq;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class PartyMenu : MonoBehaviour
    {
        [SerializeField] private Text messageText;
        [SerializeField] private GameObject partyList;
        
        private PartyMemberUI[] memberSlots;
        private List<int> orderOfPokemon;
        [SerializeField] int selection = -1;
        
        public void Init()
        {
            memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        }

        public void SetPartyData(PokemonParty partyPokemon)
        {
            orderOfPokemon = partyPokemon.GetCurrentBattleOrder();
            
            var slotAndPokemon = orderOfPokemon.Zip(memberSlots, (p, s) => new {p, s});
            foreach (var pair in slotAndPokemon)
            {
                pair.s.SetData(partyPokemon.Party[pair.p]);
                pair.s.gameObject.SetActive(true);
            }

            messageText.text = "Choose a Pokemon.";
        }

        public void EnablePartyMenu(bool enable)
        {
            partyList.SetActive(enable);
            SetSelected(0);
        }

        public void SetSelected(int newSelection)
        {
            if (selection == newSelection) return;
            
            if(selection >= 0) memberSlots[selection].SetSelected(false);
            memberSlots[newSelection].SetSelected(true);
            selection = newSelection;
        }

        public void SetMessageText(string message)
        {
            messageText.text = message;
        }
    }
}