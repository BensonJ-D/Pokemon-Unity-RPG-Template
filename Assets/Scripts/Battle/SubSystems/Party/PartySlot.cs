using PokemonScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.SubSystems.Party
{
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

        private bool _fainted;
        public void SetData(Pokemon pokemon)
        {
            _fainted = pokemon.CurrentHp <= 0;
            nameText.text = pokemon.Base.Species;
            levelText.text = pokemon.Level.ToString();
            hpBar.Setup(pokemon);
        }

        public void SetSelected(bool selected)
        {
            if(_fainted) { image.sprite = selected ? selectedFaintedSprite : notSelectedFaintedSprite; } 
            else { image.sprite = selected ? selectedSprite : notSelectedSprite; }
        }
    }
}
