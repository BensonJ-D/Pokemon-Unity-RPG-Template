using System.Collections;
using System.Utilities.Input;
using Characters.Party.PokemonParty;
using Menus.Inventory;
using Menus.InventoryMenu;
using MyBox;
using UnityEngine;

namespace Functional_Tests.Inventory_Menu_Test
{
    public class InventoryMenuTestRunner : MonoBehaviour
    {
        [Separator("Test data")] 
        [SerializeField] private InventoryMenu menu;
        [SerializeField] private Characters.Inventory.Inventory exampleInventory;
        [SerializeField] private PokemonParty examplePokemon;
    
        // Start is called before the first frame update
        private void Start()
        {
            examplePokemon.Initialise();
            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            yield return menu.OpenWindow(exampleInventory, examplePokemon);
            yield return menu.RunWindow();
        }
    }
}
