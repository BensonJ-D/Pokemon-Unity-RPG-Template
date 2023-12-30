using System;
using Battle.Controller;
using Characters.Inventories;
using Characters.Party.PokemonParty;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Players
{
    [Serializable]
    public class Player
    {
        [SerializeField] protected string name;
        [SerializeField] protected ControllerType controllerType;
        [SerializeField] protected PokemonParty party;
        [SerializeField] protected Inventory inventory;

        public string Name => name;
        public ControllerType ControllerType => controllerType;
        public PokemonParty Party => party;
        public Inventory Inventory => inventory;
    }
}