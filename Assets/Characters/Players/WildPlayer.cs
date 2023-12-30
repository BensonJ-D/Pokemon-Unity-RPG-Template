using System;
using System.Collections.Generic;
using Battle.Controller;
using Characters.Inventories;
using Characters.Monsters;
using Characters.Party.PokemonParty;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Players
{
    [Serializable]
    public class WildPlayer : Player
    {
        public WildPlayer(Pokemon wildPokemon) {
            name = wildPokemon.Name;
            controllerType = ControllerType.Wild;
            
            var partyList = new List<Pokemon> { wildPokemon };
            party = new PokemonParty(partyList);

            var inventoryList = new List<InventoryData>();
            inventory = new Inventory(inventoryList);
        }
    }
}