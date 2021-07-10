using System.Collections;
using PokemonScripts;
using UnityEngine;

namespace Inventory
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;

        public string Name => itemName;
        public string Description => description;

        public abstract IEnumerator BeforeUse();
        public abstract bool ValidateUse();
        public abstract void OnUse();
        public abstract void AfterUse();
    }
}