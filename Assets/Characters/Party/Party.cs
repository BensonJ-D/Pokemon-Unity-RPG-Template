using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.Party
{
    [Serializable]
    public class Party<T>
    {
        [SerializeField]
        private List<T> _partyMembers;
        public List<T> PartyMembers => _partyMembers;

        public void SwitchPartyMembers(T first, T second)
        {
            var firstSlot = _partyMembers.IndexOf(first);
            var secondSlot = _partyMembers.IndexOf(second);
            
            _partyMembers[firstSlot] = second;
            _partyMembers[secondSlot] = first;
        }
    }
}