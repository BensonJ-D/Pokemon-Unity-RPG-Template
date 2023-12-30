using System.Collections;
using Characters.Monsters;
using UnityEngine;

namespace Characters.Inventories
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool consumable;

        public string Name => itemName;
        public string Description => description;
        public Sprite Icon => icon;
        public bool Consumable => consumable;

        public abstract IEnumerator BeforeUse();
        public abstract ItemUseValidation ValidateUse(Pokemon target);
        public abstract IEnumerator OnUse(Pokemon target);
        public abstract void AfterUse();
    }
}