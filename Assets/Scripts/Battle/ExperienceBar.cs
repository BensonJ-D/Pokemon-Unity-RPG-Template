﻿using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class ExperienceBar : MonoBehaviour
    {
        [SerializeField] private Text totalExperienceLabel;
        [SerializeField] private Text nextLevelExperienceLabel;
        [SerializeField] private Image experienceImage;

        public float CurrentExperience { get; private set; }
        public int BaseLevelExperience { get; set; }
        public int NextLevelExperience { get; set; }

        public void Setup(Pokemon pokemon)
        {
            SetExp(pokemon.BaseLevelExperience, pokemon.CurrentExperience, pokemon.NextLevelExperience);
        }
        
        public void SetExp(float exp) { SetExp(BaseLevelExperience, exp, NextLevelExperience); }

        private void SetExp(int baseLevelExperience, float exp, int nextLevelExperience)
        {
            CurrentExperience = exp;
            BaseLevelExperience = baseLevelExperience;
            NextLevelExperience = nextLevelExperience;
            totalExperienceLabel.text = $"{Mathf.Round(CurrentExperience)}";
            nextLevelExperienceLabel.text = $"{NextLevelExperience - CurrentExperience}";
            
            var expNormalise = Mathf.Clamp((CurrentExperience - BaseLevelExperience) / (NextLevelExperience - BaseLevelExperience), 0.0f, 1.0f);
            experienceImage.transform.localScale = new Vector3(expNormalise, 1f, 1f);
        }
    }
}