using System;
using Battle.Controller;
using Characters.Party.PokemonParty;
using UnityEngine;

namespace Characters.Player
{
    [Serializable]
    public class Player
    {
        [SerializeField] protected string name;
        [SerializeField] protected ControllerType controllerType;
        [SerializeField] protected PokemonParty party;
        [SerializeField] protected Inventory.Inventory inventory;
        
        public string Name => name;
        public ControllerType ControllerType => controllerType;
        public PokemonParty Party => party;
        public Inventory.Inventory Inventory => inventory;
    }
}