using System.Collections;
using System.Collections.Generic;
using System.Transition;
using Battle;
using Characters.Inventory;
using Characters.Party.PokemonParty;
using Characters.Player;
using MyBox;
using UnityEngine;

namespace Functional_Tests.Wild_Battle_Test
{
    public class WildBattleTestRunner : MonoBehaviour
    {
        [Separator("Test data")] 
        [SerializeField] private BattleWindow window;
        [SerializeField] private Player localPlayer;
        [SerializeField] private Player wildPokemonAI;
    
        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            var battlePlayers = new List<Player> {localPlayer, wildPokemonAI};
                
            StartCoroutine(TransitionController.Instance.RunTransition(Transition.BattleEnter));
            yield return TransitionController.Instance.WaitForTransitionPeak();
            yield return window.OpenWindow(battlePlayers, true);
            StartCoroutine(window.RunWindow());
        }
    }
}
