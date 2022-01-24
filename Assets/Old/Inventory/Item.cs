using System.Collections;
using Characters.Monsters;
using UnityEngine;

namespace Inventory
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        public string Name => itemName;
        public string Description => description;
        public Sprite Icon => icon;

        public abstract IEnumerator BeforeUse();
        public abstract ItemUseValidation ValidateUse(Pokemon target);
        public abstract IEnumerator OnUse(Pokemon target);
        public abstract void AfterUse();
    }
}