using System.Collections;
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
        public abstract bool ValidateUse();
        public abstract void OnUse();
        public abstract void AfterUse();
    }
}