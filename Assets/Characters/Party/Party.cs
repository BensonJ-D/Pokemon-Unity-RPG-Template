using System.Collections.Generic;
using Characters.Monsters;
using UnityEngine;

namespace Characters.Party
{
    public class Party<T> : MonoBehaviour
    {
        [SerializeField] protected List<T> partyMembers;
        
        public List<T> PartyMembers => partyMembers;
        // private List<int> BattlePokemon { get; set; }

        public Party(List<T> partyMembers)
        {
            this.partyMembers = partyMembers;
        }

        public void SwitchPartyMembers(T first, T second)
        {
            var firstSlot = partyMembers.IndexOf(first);
            var secondSlot = partyMembers.IndexOf(second);
            
            partyMembers[firstSlot] = second;
            partyMembers[secondSlot] = first;
        }
        
    }
}