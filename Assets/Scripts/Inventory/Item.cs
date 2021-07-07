using PokemonScripts;
using UnityEngine;

namespace Inventory
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private string description;

        public abstract void BeforeUse();
        public abstract bool ValidateUse();
        public abstract void OnUse();
        public abstract void AfterUse();
    }
}