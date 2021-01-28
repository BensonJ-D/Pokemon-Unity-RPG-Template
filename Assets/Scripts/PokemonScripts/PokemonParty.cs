using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PokemonScripts
{
    [Serializable]
    public class PokemonParty : MonoBehaviour
    {
        [SerializeField] private List<Pokemon> party;
        public List<Pokemon> Party => party;
        private List<int> BattlePokemon { get; set; }
        
        private void Start()
        {
            BattlePokemon = new List<int>();
            foreach (var pokemon in party)
            {
                pokemon.Initialization();
                ResetBattleOrder();
            }
        }

        public void ResetBattleOrder()
        {
            BattlePokemon.Clear();
            for(var i = 0; i < party.Count; i++)
            {
                BattlePokemon.Add(i);
            }
        }

        public Pokemon GetFirstBattleReadyPokemon()
        {
            var indexOfFirstHealthyPokemon = BattlePokemon.FindIndex(index => party[index].Hp > 0);
            if (indexOfFirstHealthyPokemon != 0) { SetPokemonToBattleLeader(indexOfFirstHealthyPokemon); }
            return party[BattlePokemon[0]];
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