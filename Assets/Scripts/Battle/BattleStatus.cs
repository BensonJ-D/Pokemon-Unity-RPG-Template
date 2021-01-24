using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class BattleStatus : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text levelText;
        [SerializeField] private HealthBar hpBar;

        public void SetData(Pokemon.Pokemon pokemon)
        {
            nameText.text = pokemon.Base.Species;
            levelText.text = pokemon.Level.ToString();
            hpBar.Setup(pokemon);
        }

        public IEnumerator UpdateHealthBar(int damageDealt)
        {
            var newHp = hpBar.Hp - damageDealt > 0 ? hpBar.Hp - damageDealt : 0;
            while(hpBar.Hp > newHp )
            {
                hpBar.SetHp(hpBar.Hp - 1);
                if(damageDealt > hpBar.MaxHp / 2) {yield return new WaitForSeconds(0.01f);}
                else {yield return new WaitForSeconds(0.05f);}
            }
        }
    }
}