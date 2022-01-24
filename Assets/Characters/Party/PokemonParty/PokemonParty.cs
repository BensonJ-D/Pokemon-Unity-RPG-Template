using System;
using System.Collections.Generic;
using Battle;
using Characters.Monsters;
using UnityEngine;

namespace Characters.Party.PokemonParty
{
    public class PokemonParty : Party<Pokemon>
    {
        private List<int> BattlePokemon { get; set; }
        
        private void Start()
        {
            BattlePokemon = new List<int>();
            foreach (var pokemon in partyMembers)
            {
                pokemon.Initialization();
                ResetBattleOrder();
            }
        }

        public void ResetBattleOrder()
        {
            BattlePokemon.Clear();
            for(var i = 0; i < partyMembers.Count; i++)
            {
                BattlePokemon.Add(i);
            }
        }

        public Pokemon GetFirstBattleReadyPokemon()
        {
            var indexOfFirstHealthyPokemon = BattlePokemon.FindIndex(index => partyMembers[index].CurrentHp > 0);
            if (indexOfFirstHealthyPokemon != 0) { SetPokemonToBattleLeader(indexOfFirstHealthyPokemon); }
            return partyMembers[BattlePokemon[0]];
        }

        public void SetPokemonToBattleLeader(int index)
        {
            var newHeadOfParty = BattlePokemon[index];
            BattlePokemon[index] = BattlePokemon[0];
            BattlePokemon[0] = newHeadOfParty;
        }
        
        public List<int> GetCurrentBattleOrder()
        {
            return BattlePokemon;
        }

        public PokemonParty(List<Pokemon> partyMembers) : base(partyMembers)
        {
        }
    }
}