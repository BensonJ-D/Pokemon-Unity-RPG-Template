using System;
using System.Collections;
using System.Collections.Generic;
using PokemonScripts;
using PokemonScripts.Conditions;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattleStatus : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private Image statusCondition;
        [SerializeField] private HealthBar hpBar;
        [SerializeField] private List<Sprite> statusIcons;

        public void SetData(Pokemon pokemon)
        {
            nameText.text = pokemon.Base.Species;
            levelText.text = pokemon.Level.ToString();
            hpBar.Setup(pokemon);
            UpdateStatus(pokemon);
        }

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
        
        public void UpdateStatus(Pokemon pokemon)
        {
            statusCondition.enabled = pokemon.PrimaryCondition != PrimaryStatusCondition.None;
            switch (pokemon.PrimaryCondition)
            {
                case PrimaryStatusCondition.None:
                    break;
                case PrimaryStatusCondition.Poison:
                    statusCondition.sprite = statusIcons[0];
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