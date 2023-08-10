using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Battle.Controller;
using Characters.Monsters;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Characters.Party.PokemonParty
{
    [Serializable]
    public class PokemonParty : Party<Pokemon>
    {
        private List<int> BattlePokemon { get; set; }
        private bool _initialised;
        
        public void Initialise()
        {
            _initialised = true;
            BattlePokemon = new List<int>();
            
            foreach (var pokemon in PartyMembers) { pokemon.Initialization(); }
            
            ResetBattleOrder();
        }

        public void ResetBattleOrder()
        {
            if (!_initialised) Initialise();
            
            BattlePokemon.Clear();
            BattlePokemon = Enumerable.Range(0, PartyMembers.Count).ToList();
        }
        
        public Pokemon GetBattleOrderedPokemon(int positionOfCombatantPokemon)
        {
            if (!_initialised) Initialise();
            return PartyMembers[BattlePokemon[positionOfCombatantPokemon]];
        }
        
        public Pokemon GetNextBattleReadyPokemon(int positionOfCombatantPokemon)
        {
            if (!_initialised) Initialise();
            
            var battleIndexOfNextHealthyPokemon = BattlePokemon.Skip(positionOfCombatantPokemon)
                .FirstIndex(index => PartyMembers[index].CurrentHp > 0);

            if (battleIndexOfNextHealthyPokemon != -1)
            {
                SwapBattlePokemonPositions(positionOfCombatantPokemon, battleIndexOfNextHealthyPokemon + positionOfCombatantPokemon);
            }
            return PartyMembers[BattlePokemon[positionOfCombatantPokemon]];
        }

        public void SwapBattlePokemonPositions(int first, int second)
        {
            if (!_initialised) Initialise();
            
            var temp = BattlePokemon[first];
            BattlePokemon[first] = BattlePokemon[second];
            BattlePokemon[second] = temp;
        }
        
        public List<Pokemon> GetCurrentBattleOrder()
        {
            if (!_initialised) Initialise();
            
            return (from index in BattlePokemon
                select PartyMembers[index]).ToList();
        }
    }
}