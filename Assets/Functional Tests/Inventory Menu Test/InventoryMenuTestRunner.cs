using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using Menus.Inventory;
using MyBox;
using PokemonScripts;
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
            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            yield return menu.OpenWindow(exampleInventory, examplePokemon);
            yield return menu.RunWindow();
        }
    }
}
