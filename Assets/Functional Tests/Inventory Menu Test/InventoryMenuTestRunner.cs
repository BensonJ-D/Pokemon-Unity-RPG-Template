using System.Collections;
using Characters.Inventories;
using Characters.Party.PokemonParty;
using Menus.InventoryMenu;
using MyBox;
using UnityEngine;
using UnityEngine.Serialization;

namespace Functional_Tests.Inventory_Menu_Test
{
    public class InventoryMenuTestRunner : MonoBehaviour
    {
        [Separator("Test data")] [SerializeField]
        private InventoryMenu menu;

        [FormerlySerializedAs("exampleInventoryImpl")] [SerializeField] private Inventory exampleInventory;
        [SerializeField] private PokemonParty examplePokemon;

        // Start is called before the first frame update
        private void Start() {
            examplePokemon.Initialise();
            StartCoroutine(Test());
        }

        private IEnumerator Test() {
            yield return menu.OpenWindow(exampleInventory, examplePokemon);
            yield return menu.RunWindow();
        }
    }
}