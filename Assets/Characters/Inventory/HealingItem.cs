using System.Collections;
using System.Collections.Generic;
using Characters.Monsters;
using PokemonScripts.Conditions;
using UnityEngine;

namespace Characters.Inventory
{
    [CreateAssetMenu(fileName = "Healing Item", menuName = "Item/Create new healing item", order = 0)]
    public class HealingItem : Item
    {
        [SerializeField] private int amountHealed;
        // [SerializeField] private List<PrimaryStatusCondition> primaryStatusConditionsHealed;
        // [SerializeField] private List<SecondaryStatusCondition> secondaryStatusConditionsHealed;

        public override IEnumerator BeforeUse() { yield return null; }

        public override ItemUseValidation ValidateUse(Pokemon target)
        {
            if (target.CurrentHp == target.MaxHp())
            {
                return new ItemUseValidation{
                    Successful = false, 
                    ResponseMessage = "Pokemon's HP is already full!"
                };
            }

            return new ItemUseValidation{
                Successful = true, 
                ResponseMessage = $"You used {Name} on {target.Name}"
            };
        }

        public override IEnumerator OnUse(Pokemon target)
        {
            yield return target.UpdateHealth(amountHealed);
        }

        public override void AfterUse() {}
    }
}