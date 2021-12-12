using Battle;
using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;
using VFX;

public class SummaryMenu : SceneWindow
{
    [SerializeField] private Image pokemonSprite;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Text attackText;
    [SerializeField] private Text defenceText;
    [SerializeField] private Text spAtkText;
    [SerializeField] private Text spDefText;
    [SerializeField] private Text speedText;
    [SerializeField] private ExperienceBar expBar;

    public override void Init()
    {
        Scene = Scene.SummaryView;
        base.Init();
    }

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonSprite.sprite = pokemon.Base.FrontSprite;
        healthBar.Setup(pokemon);
        attackText.text = pokemon.Attack().ToString();
        defenceText.text = pokemon.Defence().ToString();
        spAtkText.text = pokemon.SpAttack().ToString();
        spDefText.text = pokemon.SpDefence().ToString();
        speedText.text = pokemon.Speed().ToString();
        expBar.Setup(pokemon);
    }
}
