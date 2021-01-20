using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Healthbar hpBar;

    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        hpBar.HP = pokemon.HP;
        hpBar.MaxHP = pokemon.MaxHP;
    }
}