using System.Collections;
using Characters.Party.PokemonParty;
using Menus.Inventory;
using MyBox;
using UnityEngine;

namespace Functional_Tests.Inventory_Menu_Test
{
    public class InventoryMenuTestRunner : MonoBehaviour
    {
        [Separator("Test data")] 
        [SerializeField] private InventoryMenu menu;
        [SerializeField] private Inventory.Inventory exampleInventory;
        [SerializeField] private PokemonParty examplePokemon;
    
        // Start is called before the first frame update
        private void Start()
        {
            examplePokemon.Initialization();
            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            yield return menu.OpenWindow(exampleInventory, examplePokemon);
            yield return menu.RunWindow();
        }
    }
}
