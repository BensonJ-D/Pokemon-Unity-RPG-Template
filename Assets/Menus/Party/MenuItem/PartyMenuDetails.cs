using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Battle;
using MyBox;
using PokemonScripts;
using PokemonScripts.Conditions;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.Party.MenuItem
{
    public class PartyMenuDetails : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image statusCondition;
        [SerializeField] private HealthBar hpBar;

        [SerializeField] private bool hasExperienceBar;
        [ConditionalField(nameof(hasExperienceBar))] [SerializeField] private ExperienceBar expBar;

        public void SetData(Pokemon pokemon)
        {
            nameText.text = pokemon.Base.Species;
            levelText.text = pokemon.Level.ToString();
            hpBar.Setup(pokemon);
         
            if(hasExperienceBar) expBar.Setup(pokemon);
            // UpdateStatus(pokemon);
        }

        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
        public IEnumerator UpdateHealthBar(DamageDetails dmgDetails)
        {
            var targetHp = hpBar.Hp - dmgDetails.DamageDealt > 0 ? hpBar.Hp - dmgDetails.DamageDealt : 0;
            while(hpBar.Hp > targetHp)
            {
                var newHp = hpBar.Hp - (hpBar.MaxHp / 100f) > 0 ? hpBar.Hp - (hpBar.MaxHp / 100f) : 0;
                hpBar.SetHp(newHp);
                yield return new WaitForSeconds(0.05f / dmgDetails.Multiplier);
            }
        }
        
        public void ResetExperienceBar(Pokemon pokemon)
        {
            expBar.Setup(pokemon);
        }
        
        public IEnumerator UpdateExperienceBar(int experienceGained)
        {
            var targetExp = expBar.CurrentExperience + experienceGained;
            while(expBar.CurrentExperience < targetExp)
            {
                var expStep = (expBar.NextLevelExperience - expBar.BaseLevelExperience) / 100f;
                var newExp = expBar.CurrentExperience + expStep;
                expBar.SetExp(newExp);
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        public void UpdateStatus(Pokemon pokemon)
        {
            statusCondition.enabled = pokemon.PrimaryCondition != PrimaryStatusCondition.None;
            switch (pokemon.PrimaryCondition)
            {
                case PrimaryStatusCondition.None:
                    break;
                case PrimaryStatusCondition.Poison:
                    break;
                case PrimaryStatusCondition.Burn:
                    break;
                case PrimaryStatusCondition.Paralyse:
                    break;
                case PrimaryStatusCondition.Freeze:
                    break;
                case PrimaryStatusCondition.Sleep:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}