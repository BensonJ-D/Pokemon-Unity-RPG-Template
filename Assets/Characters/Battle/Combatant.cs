using System.Collections;
using Battle;
using Characters.Monsters;
using Characters.UI;
using Menus.Party.MenuItem;
using UnityEngine;
using UnityEngine.UI;

namespace Characters.Battle
{
    public class Combatant : MonoBehaviour
    {
        [SerializeField] protected bool displayFront;
        [SerializeField] private Animator animator;
        [SerializeField] protected CharacterStatus pokemonStatus;

        private static readonly int ReturnToIdle = Animator.StringToHash("ReturnToIdle");

        private IEnumerator PlayAnimation(string animationName, bool reset = true)
        {
            animator.ResetTrigger(ReturnToIdle);
            animator.Play(animationName, 0);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            
            if(reset) animator.SetTrigger(ReturnToIdle);
        }
        
        public IEnumerator PlayEnterAnimation()
        {
            var animationName = displayFront ? "Enter Right" : "Enter Left";
            yield return PlayAnimation(animationName);
        }
        
        public IEnumerator PlayDamageAnimation()
        {
            const string animationName = "Damage Blink";
            yield return PlayAnimation(animationName);
        }
        
        public IEnumerator PlayBasicHitAnimation()
        {
            var animationName = displayFront ? "Basic Attack Front" : "Basic Attack Back";
            yield return PlayAnimation(animationName);
        }
        
        public IEnumerator PlayFaintAnimation()
        {
            const string animationName = "Faint";
            yield return PlayAnimation(animationName, false);
        }

        public IEnumerator ResetAnimation()
        {
            animator.SetTrigger(ReturnToIdle);
            
            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public void UpdateStatus()
        {
            // hud.UpdateStatus(Pokemon);
        }
    }
}