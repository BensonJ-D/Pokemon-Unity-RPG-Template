using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePokemon : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;
    [SerializeField] private bool isPlayerUnit;
    
    public Pokemon Pokemon { get; set; }

    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        GetComponent<Image>().sprite = isPlayerUnit ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite;
    }
}
