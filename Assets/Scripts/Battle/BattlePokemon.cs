﻿using System.Collections;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattlePokemon : MonoBehaviour
    {
        [SerializeField] private bool displayFront;
        [SerializeField] private Image image;
        [SerializeField] private Animator animator;
        [SerializeField] private BattleStatus hud;
        
        private static readonly int Reset = Animator.StringToHash("Reset");
        public Pokemon Pokemon { get; set; }
        
        public void Setup(Pokemon pokemon)
        {
            Pokemon = pokemon;
            hud.SetData(pokemon);
            
            image.sprite = displayFront ? Pokemon.Base.FrontSprite : Pokemon.Base.BackSprite;
        }

        private IEnumerator PlayAnimation(string animationName, bool reset = true)
        {
            animator.Play(animationName, 0);

            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            
            if(reset) animator.SetTrigger(Reset);
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
            animator.SetTrigger(Reset);
            
            yield return null;
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        public IEnumerator UpdateHealth(DamageDetails damageDetails)
        {
            Pokemon.CurrentHp = damageDetails.Fainted ? 0 : Pokemon.CurrentHp - damageDetails.DamageDealt;
            yield return hud.UpdateHealthBar(damageDetails);
        }
    }
}