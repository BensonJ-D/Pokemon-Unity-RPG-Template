using System.Collections;
using System.Linq;
using Characters.Party.PokemonParty;
using Menus.Party;
using MyBox;
using UnityEngine;

namespace Functional_Tests.Party_Menu_Test
{
    public class PartyMenuTestRunner : MonoBehaviour
    {
        [Separator("Test data")] 
        [SerializeField] private PartyMenu menu;
        [SerializeField] private PokemonParty examplePokemon;
    
        // Start is called before the first frame update
        private void Start()
        {
            examplePokemon.PartyMembers.Last().CurrentHp = 0;
            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            yield return menu.OpenWindow(examplePokemon);
            yield return menu.RunWindow();
        }
    }
}
