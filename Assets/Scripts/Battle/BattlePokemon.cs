﻿﻿using System;
 using System.Collections;
using PokemonScripts;
 using PokemonScripts.Conditions;
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
        
        private LevelUpWindow LevelUpWindow { get; set; }

        private static readonly int Reset = Animator.StringToHash("Reset");
        public Pokemon Pokemon { get; private set; }

        public void Setup(Pokemon pokemon)
        {
            animator.ResetTrigger(Reset);
            Pokemon = pokemon;
            hud.SetData(pokemon);
            
            image.sprite = displayFront ? Pokemon.Base.FrontSprite : Pokemon.Base.BackSprite;
            LevelUpWindow = FindObjectOfType<LevelUpWindow>();
        }

        private IEnumerator PlayAnimation(string animationName, bool reset = true)
        {
            animator.Play(animationName, 0);

            yield return null;
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
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

        public IEnumerator UpdateExperience(int experienceGain)
        {
            while (experienceGain > 0)
            {
                var expToLevel = Pokemon.NextLevelExperience - Pokemon.CurrentExperience;
                var expStep = experienceGain;
                if (expStep > expToLevel)
                {
                    expStep = expToLevel;
                }

                experienceGain -= expStep;
                yield return hud.UpdateExperienceBar(expStep);
                Pokemon.CurrentExperience += expStep;
                var levelUp = Pokemon.CheckForLevel();

                if (!levelUp) continue;
                hud.SetData(Pokemon);
                yield return new WaitForSeconds(1f);
                yield return LevelUpWindow.ShowWindow(Pokemon.GetStats(Pokemon.Level - 1), Pokemon.GetStats());
            }
        }

        public void UpdateStatus()
        {
            hud.UpdateStatus(this.Pokemon);
        }
    }
}