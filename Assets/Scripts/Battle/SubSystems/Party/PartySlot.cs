using System.Collections;
using System.Collections.Generic;
using Battle;
using PokemonScripts;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PartySlot : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HealthBar hpBar;
    [SerializeField] private Sprite notSelectedSprite;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite notSelectedFaintedSprite;
    [SerializeField] private Sprite selectedFaintedSprite;
    [SerializeField] private Image image;

    private Pokemon _pokemon;
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Species;
        levelText.text = pokemon.Level.ToString();
        hpBar.Setup(pokemon);
    }

    public void SetSelected(bool selected)
    {
        if(_pokemon.Hp <= 0) { image.sprite = selected ? selectedFaintedSprite : notSelectedFaintedSprite; } 
        else { image.sprite = selected ? selectedSprite : notSelectedSprite; }
    }
}
