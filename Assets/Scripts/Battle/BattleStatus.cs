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

        public IEnumerator UpdateHealthBar(Pokemon.DamageDetails damageDetails)
        {
            var newHp = hpBar.Hp - damageDetails.DamageDealt > 0 ? hpBar.Hp - damageDetails.DamageDealt : 0;
            while(hpBar.Hp > newHp )
            {
                hpBar.SetHp(hpBar.Hp - 1);
                yield return new WaitForSeconds(0.05f / damageDetails.Multiplier);
            }
        }
    }
}