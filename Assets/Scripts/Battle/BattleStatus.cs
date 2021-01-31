using System.Collections;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattleStatus : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private HealthBar hpBar;

        public void SetData(Pokemon pokemon)
        {
            nameText.text = pokemon.Base.Species;
            levelText.text = pokemon.Level.ToString();
            hpBar.Setup(pokemon);
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
    }
}