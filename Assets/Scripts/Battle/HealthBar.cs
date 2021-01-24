using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject health;
        [SerializeField] private Text hpLabel;
        [SerializeField] private Text maxHpLabel;

        private Image _healthImage;
        private Gradient _gradient;
        public int Hp { get; private set; }
        public int MaxHp { get; set; }

        public void Setup(Pokemon.Pokemon pokemon)
        {
            _gradient = new Gradient();

            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            GradientColorKey[] colorKey = new GradientColorKey[5];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0.2f;
            colorKey[1].color = Color.yellow;
            colorKey[1].time = 0.2f;
            colorKey[2].color = Color.yellow;
            colorKey[2].time = 0.5f;
            colorKey[3].color = new Color(24 / 255f, 173 / 255f, 72 / 255f);
            colorKey[3].time = 0.5f;
            colorKey[4].color = new Color(24 / 255f, 173 / 255f, 72 / 255f);
            colorKey[4].time = 1.0f;

            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            _gradient.SetKeys(colorKey, alphaKey);
            _healthImage = health.GetComponent<Image>();
            
            SetHp(pokemon.Hp, pokemon.MaxHp);
        }

        public void SetHp(int hp) { SetHp(hp, MaxHp); }

        private void SetHp(int hp, int maxHp)
        {
            Hp = hp;
            MaxHp = maxHp;
            hpLabel.text = Hp.ToString();
            maxHpLabel.text = MaxHp.ToString();
            
            var hpNormalise = (float) Hp / MaxHp;
            health.transform.localScale = new Vector3(hpNormalise, 1f, 1f);
            _healthImage.color = _gradient.Evaluate(hpNormalise);
        }
    }
}