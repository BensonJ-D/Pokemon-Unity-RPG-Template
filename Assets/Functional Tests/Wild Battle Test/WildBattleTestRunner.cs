using System.Collections;
using System.Collections.Generic;
using System.Transition;
using System.Utilities.Tasks;
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
                
            StartCoroutine(TransitionController.RunTransition(Transition.BattleEnter));
            yield return TransitionController.WaitForTransitionPeak;
            yield return window.OpenWindow(battlePlayers, true);
            
            var task = new Task(window.RunWindow());
            yield return new WaitWhile(() => task.Running);
            
            StartCoroutine(TransitionController.RunTransition(Transition.BattleEnter));
            yield return null;
            yield return TransitionController.WaitForTransitionPeak;
            yield return window.CloseWindow();
            yield return TransitionController.WaitForTransitionCompletion;
            
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}
