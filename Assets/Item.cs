using PokemonScripts;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Item", menuName = "Create Item", order = 0)]
    public class Item : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private string description;

        public virtual bool ValidUse() { return true; }
        public virtual void OnUse() { }
        public virtual void OnUse(Pokemon target) { }
    }
}