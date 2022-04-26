using System;
using System.Collections.Generic;
using Battle;
using Characters.Monsters;
using UnityEditor;
using UnityEngine;

namespace Characters.Party.PokemonParty
{
    [Serializable]
    public class PokemonParty : Party<Pokemon>
    {
        private List<int> BattlePokemon { get; set; }
        
        public void Initialization()
        {
            BattlePokemon = new List<int>();
            foreach (var pokemon in PartyMembers)
            {
                pokemon.Initialization();
                ResetBattleOrder();
            }
        }

        public void ResetBattleOrder()
        {
            BattlePokemon.Clear();
            for(var i = 0; i < PartyMembers.Count; i++)
            {
                BattlePokemon.Add(i);
            }
        }

        public Pokemon GetFirstBattleReadyPokemon()
        {
            var indexOfFirstHealthyPokemon = BattlePokemon.FindIndex(index => PartyMembers[index].CurrentHp > 0);
            if (indexOfFirstHealthyPokemon != 0) { SetPokemonToBattleLeader(indexOfFirstHealthyPokemon); }
            return PartyMembers[BattlePokemon[0]];
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
    }
}