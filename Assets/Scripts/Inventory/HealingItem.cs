using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using PokemonScripts.Conditions;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Healing Item", menuName = "Item/Create new healing item", order = 0)]
    public class HealingItem : Item
    {
        [SerializeField] private int amountHealed;
        [SerializeField] private List<PrimaryStatusCondition> primaryStatusConditionsHealed;
        [SerializeField] private List<SecondaryStatusCondition> secondaryStatusConditionsHealed;

        private Pokemon target;

        public override IEnumerator BeforeUse() { yield return null; }
        public override bool ValidateUse() { return true; }
        public override void OnUse() {}
        public override void AfterUse() {}
    }
}